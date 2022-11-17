using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Map;
using NijiDive.Controls;
using NijiDive.Health;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Mob : Entity, IDamageable, IBounceable
    {
        public UnityEvent<Mob, GameObject, DamageType> OnDeath;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] protected DamageType vulnerableTypes;
        [SerializeField] protected float bounceSpeed = 10f;
        [Space]
        [SerializeField] private CollisionData collisions;

        #region Properties
        protected Rigidbody2D Rb2d { get; private set; }
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
        public Vector2 velocity { get => Rb2d.velocity; set => Rb2d.velocity = value; }
        // Cleaner way to update a single axis of velocity
        public void SetVelocityX(float x) => Rb2d.velocity = new Vector2(x, Rb2d.velocity.y);
        public void SetVelocityY(float y) => Rb2d.velocity = new Vector2(Rb2d.velocity.x, y);
        #endregion

        protected Control[] controls;
        private Bounds groundCheckBounds, edgeCheckBounds, wallCheckBounds, ceilingCheckBounds;

        /// <summary>
        /// Override and set <see cref="controls"/> before calling to initialize them
        /// </summary>
        protected virtual void Awake()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError($"No {typeof(SpriteRenderer)} assigned", this);
                enabled = false;
            }

            Hitbox = GetComponent<Collider2D>();
            Rb2d = GetComponent<Rigidbody2D>();

            foreach (var control in controls)
            {
                control.mob = this;
                control.Awake();
                control.Start();
            }

            Health.Reset();
            OnDeath.AddListener(Death);
        }

        protected void FixedUpdate(InputData inputs)
        {
            UpdateCollisions(inputs.lStick.x);

            var dot = Vector2.Dot(transform.right, velocity.normalized);
            if (Mathf.Abs(dot) > 0.1f) spriteRenderer.flipX = dot < 0f;

            LastInputs = inputs;
            foreach (var control in controls) control.FixedUpdate();
        }

        public virtual bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point)
        {
            var canDamage = vulnerableTypes.IsVulnerableTo(damageType);
            if (canDamage)
            {
                Bounce(bounceSpeed);
                Bounce(sourceObject, bounceSpeed);
                Health.LoseHealth(damage);
                if (Health.IsEmpty) OnDeath?.Invoke(this, sourceObject, damageType);
            }

            return canDamage;
        }

        public virtual void Bounce(float velocity) => SetVelocityY(velocity);

        private void Bounce(GameObject other, float velocity)
        {
            var bounceable = other.GetComponent<IBounceable>();
            if (bounceable != null) bounceable.Bounce(velocity);
        }

        public void AddForce(Vector2 force) => Rb2d.AddForce(force);

        public T GetControlType<T>() where T : Control
        {
            foreach (var type in controls) if (type is T t) return t;

            return default;
        }

        #region Collision Checks
        private void UpdateCollisions(float localRightInput)
        {
            LastGroundCheck = GroundCheck();
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
            var collision = Physics2D.OverlapBox(boxPos, boxSize, 0f, MapManager.singleton.GroundMask);
            return collision;
        }

        /// <summary>
        /// Checks for ground below the collider using <see cref="groundCollisionWidthScaler"/> and <see cref="maxGroundDistance"/>
        /// </summary>
        /// <returns>Collider if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private Collider2D GroundCheck()
        {
            var boxPos = Rb2d.position + (collisions.maxGroundDistance / 2f * -(Vector2)transform.up);
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
            var velocityR = Vector3.Project(velocity, transform.right).magnitude;
            if (localRightDirection == 0f) localRightDirection = velocityR;

            if (localRightDirection == 0f)
            {
                edgeCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return null;
            }

            var hitboxSizeX = Hitbox.bounds.size.x;
            var dir = localRightDirection > 0f ? 1f : -1f;
            var boxPos = Rb2d.position + (dir * ((hitboxSizeX + collisions.maxGroundDistance) / 2f + collisions.edgeCollisionOffset) * (Vector2)transform.right) + (collisions.maxGroundDistance / 2f * -(Vector2)transform.up);
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
            var velocityR = Vector3.Project(velocity, transform.right).magnitude;
            if (localRightDirection == 0f) localRightDirection = velocityR;

            if (localRightDirection == 0f)
            {
                wallCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return null;
            }

            var hitboxSize = Hitbox.bounds.size;
            var dir = localRightDirection > 0f ? 1f : -1f;
            var boxPos = Rb2d.position + (dir * (hitboxSize.x + collisions.maxWallDistance) / 2f * (Vector2)transform.right) + (hitboxSize.y / 2f * (Vector2)transform.up);
            var boxSize = new Vector2(collisions.maxWallDistance, collisions.wallCollisionHeightScaler * hitboxSize.y);
            wallCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        private Collider2D CeilingCheck()
        {
            var hitboxSize = Hitbox.bounds.size;
            var boxPos = Rb2d.position + ((hitboxSize.y + collisions.maxCeilingDistance / 2) * (Vector2)transform.up);
            var boxSize = new Vector2(collisions.ceilingCollisionWidthScaler * hitboxSize.x, collisions.maxCeilingDistance);
            ceilingCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }
        #endregion

        public override void Pause(bool paused)
        {
            if (IsPaused == paused) return;

            enabled = !paused;
            Rb2d.simulated = enabled;
            var animator = GetComponentInChildren<Animator>();
            if (animator) animator.enabled = enabled;
            IsPaused = paused;
        }

        protected virtual void Death(Mob killedMob, GameObject sourceObject, DamageType damageType)
        {
            Destroy(gameObject);
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

        private void OnValidate()
        {
            if (spriteRenderer) enabled = true;
        }

        [Serializable]
        private class CollisionData
        {
            [Min(0f)] public float maxCeilingDistance = 0.1f, ceilingCollisionWidthScaler = 0.9f;
            [Space]
            [Min(0f)] public float maxWallDistance = 0.1f;
            [Min(0f)] public float wallCollisionHeightScaler = 0.9f;
            [Space]
            [Min(0f)] public float maxGroundDistance = 0.1f;
            [Min(0f)] public float groundCollisionWidthScaler = 0.9f, edgeCollisionOffset = 0f;

            [Header("Visualizers")]
            public Color gizmoColorNone = Color.red;
            public Color gizmoColorFound = Color.green;
            public bool showGroundCheck, showEdgeCheck, showWallCheck, showCeilingCheck;
        }

        public struct InputData
        {
            public Vector2 lStick;
            public bool actionDown, actionDownThisFrame;

            public InputData(Vector2 lStick = default, bool actionDown = false, bool actionDownThisFrame = false)
            {
                this.lStick = lStick;
                this.actionDown = actionDown;
                this.actionDownThisFrame = actionDownThisFrame;
            }
        }
    }
}