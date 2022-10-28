using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers.Map;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Controls
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Mob : MonoBehaviour
    {
        [Header("Collisions")]
        [SerializeField] [Min(0f)] private float maxGroundDistance = 0.1f;
        [SerializeField] [Min(0f)] private float groundCollisionWidthScaler = 1f, maxWallDistance = 0.1f, wallCollisionHeightScaler = 1f, maxCeilingDistance = 0.1f, ceilingCollisionWidthScaler = 1f;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showGroundCheck, showWallCheck, showCeilingCheck;

        #region Properties
        protected MapManager Map { get; private set; }
        protected Rigidbody2D Rb2d { get; private set; }
        protected Collider2D Hitbox { get; private set; }
        public Bounds GroundCheckBounds => groundCheckBounds;
        public Bounds WallCheckBounds => wallCheckBounds;
        public Bounds CeilingCheckBounds => ceilingCheckBounds;
        public Inputs LastInputs { get; private set; }
        public Vector2 GetVelocity() => Rb2d.velocity;
        // Cleaner way to update a single axis of velocity
        public void SetVelocityX(float x) => Rb2d.velocity = new Vector2(x, Rb2d.velocity.y);
        public void SetVelocityY(float y) => Rb2d.velocity = new Vector2(Rb2d.velocity.x, y);
        public bool lastGroundCheck { get; private set; }
        public bool lastWallCheck { get; private set; }
        public bool lastCeilingCheck { get; private set; }
        #endregion

        protected Control[] controls;
        private Bounds groundCheckBounds, wallCheckBounds, ceilingCheckBounds;

        /// <summary>
        /// Override and set <see cref="controls"/> before calling to initialize them
        /// </summary>
        protected virtual void Awake()
        {
            Map = FindObjectOfType<MapManager>();
            if (Map == null)
            {
                Debug.LogError($"No {typeof(MapManager)} found in scene");
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
        }

        protected void FixedUpdate(Inputs inputs)
        {
            lastGroundCheck = GroundCheck();
            lastWallCheck = WallCheck(GetVelocity().x);
            lastCeilingCheck = CeilingCheck();

            LastInputs = inputs;
            foreach (var control in controls) control.FixedUpdate();
        }

        public T GetMovingType<T>() where T : Moving
        {
            foreach (var type in controls) if (type is T t) return t;

            return default;
        }

        public T GetAttackType<T>() where T : Attacking
        {
            foreach (var type in controls) if (type is T t) return t;

            return default;
        }

        public void AddForce(Vector2 force) => Rb2d.AddForce(force);
        
        #region Collision Checks
        /// <summary>
        /// Performs <see cref="Physics2D.OverlapBox(Vector2, Vector2, float, int)"/>
        /// </summary>
        /// <param name="boxPos">Position of the collision check</param>
        /// <param name="boxSize">Size of the collision check</param>
        /// <returns>True if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private bool CollisionCheck(Vector2 boxPos, Vector2 boxSize)
        {
            var collision = Physics2D.OverlapBox(boxPos, boxSize, 0f, Map.GroundMask);
            return collision != null;
        }

        /// <summary>
        /// Checks for ground below the collider using <see cref="groundCollisionWidthScaler"/> and <see cref="maxGroundDistance"/>
        /// </summary>
        /// <returns>True if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private bool GroundCheck()
        {
            var boxPos = Rb2d.position + (maxGroundDistance / 2f * -(Vector2)transform.up);
            var boxSize = new Vector2(groundCollisionWidthScaler * Hitbox.bounds.size.x, maxGroundDistance);
            groundCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        /// <summary>
        /// Check for ground beside the collider using <see cref="maxWallDistance"/> and <see cref="wallCollisionHeightScaler"/>
        /// </summary>
        /// <param name="xDirection">Direction on the X axis the wall is checked for</param>
        /// <returns>True if the physics check collides with the <see cref="Map"/>'s ground mask</returns>
        private bool WallCheck(float xDirection)
        {
            if (xDirection * Rb2d.velocity.x < 0f || xDirection == 0f) xDirection = Rb2d.velocity.x;

            if (xDirection == 0f)
            {
                wallCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return true;
            }

            var hitboxSize = Hitbox.bounds.size;
            var dir = xDirection > 0f ? 1f : -1f;
            var boxPos = Rb2d.position + (dir * (hitboxSize.x + maxWallDistance) / 2f * (Vector2)transform.right) + (hitboxSize.y / 2f * (Vector2)transform.up);
            var boxSize = new Vector2(maxWallDistance, wallCollisionHeightScaler * hitboxSize.y);
            wallCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

        private bool CeilingCheck()
        {
            var hitboxSize = Hitbox.bounds.size;
            var boxPos = Rb2d.position + ((hitboxSize.y + maxCeilingDistance / 2) * (Vector2)transform.up);
            var boxSize = new Vector2(ceilingCollisionWidthScaler * hitboxSize.x, maxCeilingDistance);
            ceilingCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (showGroundCheck)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(groundCheckBounds.center, groundCheckBounds.size);
            }
            if (showWallCheck)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(wallCheckBounds.center, wallCheckBounds.size);
            }
            if (showCeilingCheck)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(ceilingCheckBounds.center, ceilingCheckBounds.size);
            }
        }

        public struct Inputs
        {
            public Vector2 lStick;
            public bool actionDown, actionDownThisFrame;

            public Inputs(Vector2 lStick, bool actionDown, bool actionDownThisFrame)
            {
                this.lStick = lStick;
                this.actionDown = actionDown;
                this.actionDownThisFrame = actionDownThisFrame;
            }
        }
    }
}