using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class Jumping : Moving
    {
        [SerializeField] [Min(0f)] private float jumpForce = 10f, variableGravityForce = 5f, jumpBufferTime = 0.25f, coyoteTime = 0.25f, minJumpInterval = 0.5f, maxJumpSpeed = 10f, maxFallSpeed = 10f;
        [Space]
        public UnityEvent OnJump;
        public UnityEvent OnLand;

        public float LastTimeGrounded { get; private set; }
        /// <summary>
        /// True within <see cref="coyoteTime"/> seconds of <see cref="GroundCheck"/> finding the ground
        /// </summary>
        public bool IsOnGround => Time.time - LastTimeGrounded <= coyoteTime;
        /// <summary>
        /// True only when <see cref="GroundCheck"/> is finding the ground
        /// </summary>
        public bool IsOnGroundRaw => Time.time - LastTimeGrounded <= Time.fixedDeltaTime;
        public bool JumpHeld { get; private set; }

        private Coroutine jumpBuffering;

        public override void FixedUpdate()
        {
            if (mob.LastGroundCheck)
            {
                if (Time.time - LastTimeGrounded > 2 * Time.fixedDeltaTime) OnLand?.Invoke();
                LastTimeGrounded = Time.time;
            }

            if (mob.LastInputs.actionDownThisFrame) TryJump();
            AddVariableGravity(mob.LastInputs.actionDown);
            ClampVerticalSpeed();
        }

        /// <summary>
        /// Method for derived classes to queue a jump. Will only queue one jump at a time, hence the "Try"
        /// </summary>
        protected void TryJump()
        {
            if (mob.LastGroundCheck) Jump();
            else if (jumpBuffering == null) jumpBuffering = mob.StartCoroutine(JumpBufferRoutine());
        }

        /// <summary>
        /// Routine which tries to jump every fixed update for <see cref="jumpBufferTime"/> seconds
        /// </summary>
        private IEnumerator JumpBufferRoutine()
        {
            var startTime = Time.time;

            while (Time.time - startTime <= jumpBufferTime && mob.isActiveAndEnabled)
            {
                if (IsOnGround)
                {
                    Jump();
                    break;
                }

                yield return new WaitForFixedUpdate();
            }

            startTime = Time.time;
            yield return new WaitUntil(() => !IsOnGround || Time.time - startTime > minJumpInterval || !mob.isActiveAndEnabled);
            jumpBuffering = null;
        }

        /// <summary>
        /// Adds <see cref="jumpForce"/> to <see cref="Rb2d"/>'s Y axis velocity
        /// </summary>
        private void Jump()
        {
            JumpHeld = true;
            mob.SetVelocityY(jumpForce);

            OnJump?.Invoke();
        }

        /// <summary>
        /// Adds <see cref="variableGravityForce"/> downwards to <see cref="Rb2d"/>'s Y axis velocity if jump is released in the air
        /// </summary>
        /// <param name="jumpDown">True if the jump command is enabled</param>
        protected void AddVariableGravity(bool jumpDown)
        {
            if (!jumpDown) JumpHeld = false;
            if (JumpHeld || jumpDown || IsOnGround) return;

            var gravityForce = variableGravityForce * Vector2.down;
            mob.AddForce(gravityForce);
        }

        /// <summary>
        /// Clamps <see cref="Rb2d"/>'s Y velocity to -<see cref="maxFallSpeed"/>
        /// </summary>
        protected void ClampVerticalSpeed()
        {
            mob.SetVelocityY(Mathf.Clamp(mob.velocity.y, -maxFallSpeed, maxJumpSpeed));
        }
    }
}