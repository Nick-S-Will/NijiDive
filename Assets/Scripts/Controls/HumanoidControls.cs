using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers;

namespace NijiDive.Controls
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class HumanoidControls : MonoBehaviour
    {
        [Header("Moving")]
        [SerializeField] private float moveForce = 5f;
        [Tooltip("The force multiplier added when starting to move the opposite way")]
        [SerializeField] [Min(1f)] private float reverseForceMultiplier = 1f;
        [SerializeField] [Range(0f, 1f)] private float extraDeceleration = 0f;

        [Header("Jumping")]
        [SerializeField] [Min(0f)] private float jumpForce = 10f;
        [SerializeField] [Min(0f)] private float variableGravityForce = 5f, jumpBufferTime = 0.25f, coyoteTime = 0.25f, minJumpInterval = 0.5f;

        [Header("Falling")]
        [SerializeField] [Min(0f)] private float maxFallSpeed = 3f;

        [Header("Collisions")]
        [SerializeField] [Min(0f)] private float maxGroundDistance = 0.1f;
        [SerializeField] [Min(0f)] private float groundCollisionWidthScaler = 1f, maxWallDistance = 0.1f, wallCollisionHeightScaler = 1f;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showGroundCheck, showWallCheck;

        [Header("Feature Toggles")]
        [SerializeField] private bool jumpBuffering = true;
        [SerializeField] private bool coyoteTiming = true;

        protected MapManager Map { get; private set; }
        protected Rigidbody2D Rb2d { get; private set; }
        protected Collider2D Hitbox { get; private set; }
        protected float LastTimeGrounded { get; private set; }
        /// <summary>
        /// True within <see cref="coyoteTime"/> seconds of <see cref="GroundCheck"/> finding the ground
        /// </summary>
        protected bool OnGround => coyoteTiming ? Time.time - LastTimeGrounded <= coyoteTime : Time.time - LastTimeGrounded <= Time.fixedDeltaTime;
        protected bool JumpHeld { get; private set; }

        private Coroutine jumpTry;
        private Bounds groundCheckBounds, wallCheckBounds;

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
        }

        protected virtual void FixedUpdate()
        {
            if (GroundCheck()) LastTimeGrounded = Time.time;
        }

        // Cleaner way to update a single axis of velocity
        private void SetVelocityX(float x) => Rb2d.velocity = new Vector2(x, Rb2d.velocity.y);
        private void SetVelocityY(float y) => Rb2d.velocity = new Vector2(Rb2d.velocity.x, y);

        /// <summary>
        /// Updates <see cref="Rb2d"/>'s X axis velocity
        /// </summary>
        /// <param name="xInput">Scales the applied movement force</param>
        protected void Move(float xInput)
        {
            if (WallCheck(xInput))
            {
                SetVelocityX(0f);
            }
            else if (xInput == 0f)
            {
                Rb2d.AddForce(-extraDeceleration * Rb2d.velocity.x * Vector2.right);
            }
            else
            {
                var moveForce = xInput * this.moveForce * Vector2.right;
                if (xInput * Rb2d.velocity.x < 0f) moveForce *= reverseForceMultiplier;
                Rb2d.AddForce(moveForce);
            }
        }

        /// <summary>
        /// Method for derived classes to queue a jump. Will only queue one jump at a time, hence the "Try"
        /// </summary>
        protected void TryJump()
        {
            if (!jumpBuffering && OnGround) Jump();
            else if (jumpTry == null) jumpTry = StartCoroutine(TryJumpRoutine());
        }
        /// <summary>
        /// Routine which tries to jump every fixed update for <see cref="jumpBufferTime"/> seconds
        /// </summary>
        private IEnumerator TryJumpRoutine()
        {
            var startTime = Time.time;

            while (Time.time - startTime <= jumpBufferTime && isActiveAndEnabled)
            {
                if (OnGround)
                {
                    Jump();
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            startTime = Time.time;
            yield return new WaitUntil(() => !OnGround || Time.time - startTime > minJumpInterval || !isActiveAndEnabled);
            jumpTry = null;
        }
        /// <summary>
        /// Adds <see cref="jumpForce"/> to <see cref="Rb2d"/>'s Y axis velocity
        /// </summary>
        private void Jump()
        {
            JumpHeld = true;
            Rb2d.velocity = jumpForce * Vector2.up;
        }

        /// <summary>
        /// Adds <see cref="variableGravityForce"/> downwards to <see cref="Rb2d"/>'s Y axis velocity if jump is released in the air
        /// </summary>
        /// <param name="jumpDown">True if the jump command is enabled</param>
        protected void TryAddVariableGravity(bool jumpDown)
        {
            if (!jumpDown) JumpHeld = false;
            if (JumpHeld || jumpDown || OnGround || Rb2d.velocity.y <= 0) return;

            var gravityForce = variableGravityForce * Vector2.down;
            Rb2d.AddForce(gravityForce);
        }

        protected void ClampFallSpeed()
        {
            if (-Rb2d.velocity.y > maxFallSpeed) SetVelocityY(-maxFallSpeed);
        }

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
            var boxPos = Rb2d.position + (maxGroundDistance / 2f * Vector2.down);
            var boxSize = new Vector2(groundCollisionWidthScaler * Hitbox.bounds.size.x, maxGroundDistance);
            if (showGroundCheck) groundCheckBounds = new Bounds(boxPos, boxSize);

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
                if (showWallCheck) wallCheckBounds = new Bounds(Vector3.zero, Vector3.zero);
                return true;
            }

            var hitboxSize = Hitbox.bounds.size;
            var dir = xDirection > 0f ? 1f : -1f;
            var boxPos = Rb2d.position + (dir * (hitboxSize.x + maxWallDistance) / 2f * Vector2.right) + (hitboxSize.y / 2f * Vector2.up);
            var boxSize = new Vector2(maxWallDistance, wallCollisionHeightScaler * hitboxSize.y);
            if (showWallCheck) wallCheckBounds = new Bounds(boxPos, boxSize);

            return CollisionCheck(boxPos, boxSize);
        }

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
        }
    }
}