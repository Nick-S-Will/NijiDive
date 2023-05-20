using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Map;
using NijiDive.Controls;
using NijiDive.Controls.Attacks.Specials;
using NijiDive.Health;

namespace NijiDive.Entities.Mobs
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Mob : Entity, IDamageable, IBounceable
    {
        public UnityEvent OnLandOnGround;
        public UnityEvent<MonoBehaviour, DamageType> OnDeath;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected DamageType vulnerableTypes;
        [SerializeField] protected float bounceSpeed = 10f;
        [Space]
        [SerializeField] protected CollisionData collisions;

        #region Properties
        protected Rigidbody2D Body2d { get; private set; }
        protected Collider2D Hitbox { get; private set; }
        public Collider2D LastGroundCheck { get; private set; }
        public Collider2D LastEdgeCheck { get; private set; }
        public Collider2D LastWallCheck { get; private set; }
        public Collider2D LastCeilingCheck { get; private set; }
        public abstract HealthData Health { get; }
        public Bounds GroundCheckBounds => groundCheckBounds;
        public Bounds EdgeCheckBounds => edgeCheckBounds;
        public Bounds WallCheckBounds => wallCheckBounds;
        public Bounds CeilingCheckBounds => ceilingCheckBounds;
        public InputData LastInputs { get; private set; }
        public Vector2 Velocity { get => Body2d.velocity; set => Body2d.velocity = value; }
        // Cleaner way to update a single axis of velocity
        public void SetVelocityX(float x) => Body2d.velocity = new Vector2(x, Body2d.velocity.y);
        public void SetVelocityY(float y) => Body2d.velocity = new Vector2(Body2d.velocity.x, y);
        public bool PerformCollisionChecks { get; protected set; } = true;
        #endregion

        protected List<Control> controls = new List<Control>();
        private Bounds groundCheckBounds, edgeCheckBounds, wallCheckBounds, ceilingCheckBounds;

        public static UnityEvent<Mob, MonoBehaviour, DamageType> OnMobDeath = new UnityEvent<Mob, MonoBehaviour, DamageType>();

        /// <summary>
        /// Override and set <see cref="controls"/> before calling to initialize them
        /// </summary>
        protected virtual void Awake()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError($"No {nameof(SpriteRenderer)} assigned", this);
                enabled = false;
            }

            Hitbox = GetComponent<Collider2D>();
            Body2d = GetComponent<Rigidbody2D>();

            if ((from control in controls where control is Special select control).Count() > 1) 
            {
                Debug.LogError($"Mobs can only have up to 1 {nameof(Special)}");
                enabled = false;
            }

            foreach (var control in controls) control.Setup(this);

            Health.Reset();
            OnDeath.AddListener((killedBy, damageType) => OnMobDeath?.Invoke(this, killedBy, damageType));
            OnDeath.AddListener(Death);
        }

        protected void UseControls(InputData inputs)
        {
            if (PerformCollisionChecks) UpdateCollisionChecks(inputs.lStick.x);

            var dot = Vector2.Dot(transform.right, Velocity.normalized);
            if (Mathf.Abs(dot) > 0.1f) spriteRenderer.flipX = dot < 0f;

            LastInputs = inputs;
            foreach (var control in controls) if (control.IsEnabled) control.TryToUse();
        }

        public void ResetControls()
        {
            foreach (var control in controls) control.Reset();
        }

        public virtual void SetControls(bool enabled)
        {
            for (int i = 0; i < controls.Count; i++) controls[i].SetEnabled(enabled);
        }
        public virtual void EnableControls() => SetControls(true);
        public virtual void DisableControls() => SetControls(false);

        public virtual bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            var canDamage = vulnerableTypes.IsVulnerableTo(damageType);
            if (canDamage)
            {
                Bounce(bounceSpeed);
                Bounce(sourceBehaviour.gameObject, bounceSpeed);

                Health.LoseHealth(damage);
                if (Health.IsEmpty)
                {
                    OnDeath?.Invoke(sourceBehaviour, damageType);
                    OnMobDeath?.Invoke(this, sourceBehaviour, damageType);
                }
                else _ = Flash(spriteRenderer);
            }

            return canDamage;
        }

        public virtual void Bounce(float velocity) => SetVelocityY(velocity);

        private void Bounce(GameObject other, float velocity)
        {
            var bounceable = other.GetComponent<IBounceable>();
            if (bounceable != null) bounceable.Bounce(velocity);
        }

        public void AddForce(Vector2 force) => Body2d.AddForce(force);

        #region Controls
        public T GetControlType<T>() where T : Control
        {
            foreach (var type in controls) if (type is T t) return t;

            return null;
        }

        public void AddControlType<T>(T newControl) where T : Control
        {
            if (GetControlType<T>() == null) controls.Add(newControl);
            else Debug.LogError($"Cannot add duplicate control type {newControl.GetType()}", this);
        }
        public T RemoveControlType<T>() where T : Control
        {
            foreach (var type in controls.ToArray())
            {
                if (type is T t)
                {
                    controls.Remove(type);
                    return t;
                }
            }

            return null;
        }
        #endregion

        #region Collision Checks
        private void UpdateCollisionChecks(float localRightInput)
        {
            var lastGroundCheck = LastGroundCheck;
            LastGroundCheck = GroundCheck();
            if (LastGroundCheck && !lastGroundCheck) OnLandOnGround.Invoke();

            LastEdgeCheck = EdgeCheck(localRightInput);
            LastWallCheck = WallCheck(localRightInput);
            LastCeilingCheck = CeilingCheck();
        }

        /// <summary>
        /// Performs <see cref="Physics2D.OverlapBox(Vector2, Vector2, float, int)"/>
        /// </summary>
        /// <param name="boxPos">Position of the collision check</param>
        /// <param name="boxSize">Size of the collision check</param>
        /// <returns>Collider if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private Collider2D CollisionCheck(Vector2 boxPos, Vector2 boxSize)
        {
            if (MapManager.singleton == null) return null;

            var collision = Physics2D.OverlapBox(boxPos, boxSize, 0f, MapManager.singleton.GroundMask);
            return collision;
        }

        /// <summary>
        /// Checks for ground below the collider using <see cref="groundCollisionWidthScaler"/> and <see cref="maxGroundDistance"/>
        /// </summary>
        /// <returns>Collider if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private Collider2D GroundCheck()
        {
            var boxPos = Body2d.position + (collisions.maxGroundDistance / 2f * -(Vector2)transform.up);
            var boxSize = new Vector2(collisions.groundCollisionWidthScaler * Hitbox.bounds.size.x, collisions.maxGroundDistance);
            groundCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        /// <summary>
        /// Checks for ground below and just in front of the collider using <see cref="edgeCollisionOffset"/> and <see cref="maxGroundDistance"/>
        /// </summary>
        /// <returns>Collider if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private Collider2D EdgeCheck(float localRightDirection)
        {
            var velocityR = Vector3.Project(Velocity, transform.right).magnitude;
            if (localRightDirection == 0f) localRightDirection = velocityR;

            if (localRightDirection == 0f)
            {
                edgeCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return null;
            }

            var hitboxSizeX = Hitbox.bounds.size.x;
            var dir = localRightDirection > 0f ? 1f : -1f;
            var boxPos = Body2d.position + (dir * ((hitboxSizeX + collisions.maxGroundDistance) / 2f + collisions.edgeCollisionOffset) * (Vector2)transform.right) + (collisions.maxGroundDistance / 2f * -(Vector2)transform.up);
            var boxSize = new Vector2(collisions.maxGroundDistance, collisions.maxGroundDistance);
            edgeCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        /// <summary>
        /// Check for ground beside the collider using <see cref="maxWallDistance"/> and <see cref="wallCollisionHeightScaler"/>
        /// </summary>
        /// <param name="localRightDirection">Direction on the X axis the wall is checked for</param>
        /// <returns>Collider if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private Collider2D WallCheck(float localRightDirection)
        {
            var velocityR = Vector3.Project(Velocity, transform.right).magnitude;
            if (localRightDirection == 0f) localRightDirection = velocityR;

            if (localRightDirection == 0f)
            {
                wallCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return null;
            }

            var hitboxSize = Hitbox.bounds.size;
            var dir = localRightDirection > 0f ? 1f : -1f;
            var boxPos = Body2d.position + (dir * (hitboxSize.x + collisions.maxWallDistance) / 2f * (Vector2)transform.right) + (hitboxSize.y / 2f * (Vector2)transform.up);
            var boxSize = new Vector2(collisions.maxWallDistance, collisions.wallCollisionHeightScaler * hitboxSize.y);
            wallCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        private Collider2D CeilingCheck()
        {
            var hitboxSize = Hitbox.bounds.size;
            var boxPos = Body2d.position + ((hitboxSize.y + collisions.maxCeilingDistance / 2) * (Vector2)transform.up);
            var boxSize = new Vector2(collisions.ceilingCollisionWidthScaler * hitboxSize.x, collisions.maxCeilingDistance);
            ceilingCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }
        #endregion

        public override void Pause(bool paused)
        {
            if (IsPaused == paused) return;

            enabled = !paused;
            Body2d.simulated = enabled;
            var animator = GetComponentInChildren<Animator>();
            if (animator) animator.enabled = enabled;
            IsPaused = paused;
        }

        protected virtual void Death(MonoBehaviour sourceBehaviour, DamageType damageType)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            foreach (var control in controls) control.OnDestroy();
        }

        protected virtual void OnDrawGizmos()
        {
            if (collisions.showGroundCheck)
            {
                Gizmos.color = LastGroundCheck ? collisions.gizmoColorFound : collisions.gizmoColorNone;
                Gizmos.DrawCube(groundCheckBounds.center, groundCheckBounds.size);
            }
            if (collisions.showEdgeCheck)
            {
                Gizmos.color = LastEdgeCheck ? collisions.gizmoColorFound : collisions.gizmoColorNone;
                Gizmos.DrawCube(edgeCheckBounds.center, edgeCheckBounds.size);
            }
            if (collisions.showWallCheck)
            {
                Gizmos.color = LastWallCheck ? collisions.gizmoColorFound : collisions.gizmoColorNone;
                Gizmos.DrawCube(wallCheckBounds.center, wallCheckBounds.size);
            }
            if (collisions.showCeilingCheck)
            {
                Gizmos.color = LastCeilingCheck ? collisions.gizmoColorFound : collisions.gizmoColorNone;
                Gizmos.DrawCube(ceilingCheckBounds.center, ceilingCheckBounds.size);
            }
        }

        [Serializable]
        protected class CollisionData
        {
            [Min(0f)] public float maxCeilingDistance = 0.1f, ceilingCollisionWidthScaler = 0.9f;
            [Space]
            [Min(0f)] public float maxWallDistance = 0.1f;
            [Min(0f)] public float wallCollisionHeightScaler = 0.9f;
            [Space]
            [Min(0f)] public float maxGroundDistance = 0.1f;
            [Min(0f)] public float groundCollisionWidthScaler = 0.9f;
            [Space]
            [Min(0f)] public float edgeCollisionOffset = 0f;

            [Header("Visualizers")]
            public Color gizmoColorNone = Color.red;
            public Color gizmoColorFound = Color.green;
            public bool showCeilingCheck, showWallCheck, showGroundCheck, showEdgeCheck;
        }

        public struct InputData
        {
            public Vector2 lStick;
            public bool actionDown, actionDownThisFrame, altDown, altDownThisFrame;

            public InputData(Vector2 lStick = default, bool actionDown = false, bool actionDownThisFrame = false, bool altDown = false, bool altDownThisFrame = false)
            {
                this.lStick = lStick;
                this.actionDown = actionDown;
                this.actionDownThisFrame = actionDownThisFrame;
                this.altDown = altDown;
                this.altDownThisFrame = altDownThisFrame;
            }

            public override string ToString() => $"Direction: {lStick}, Action: {(actionDownThisFrame ? 2 : actionDown ? 1 : 0)}, Alt: {(altDownThisFrame ? 2 : altDown ? 1 : 0)}";
        }
    }
}