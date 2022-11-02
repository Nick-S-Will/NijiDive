using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class Walking : Moving
    {
        [SerializeField] private float walkForce = 5f;
        [Tooltip("The force multiplier added when starting to move the opposite way")]
        [SerializeField] [Min(1f)] private float reverseForceMultiplier = 1f;
        [SerializeField] [Min(0f)] private float extraDeceleration = 0f, minVelocity = 0.1f;
        [Space]
        public UnityEvent OnStartWalk, OnStopWalk;

        private bool wasMoving;

        public override void FixedUpdate()
        {
            Move(mob.LastInputs.lStick.x);
        }

        /// <summary>
        /// Updates <see cref="Rb2d"/>'s X axis velocity
        /// </summary>
        /// <param name="xInput">Scales the applied movement force</param>
        protected void Move(float xInput)
        {
            var xVelocity = mob.GetVelocity().x;
            if (xInput == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopWalk?.Invoke();
                
                mob.AddForce(-extraDeceleration * xVelocity * Vector2.right);
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartWalk?.Invoke();

                var moveForce = xInput * walkForce * Vector2.right;
                if (xInput * xVelocity < 0f) moveForce *= reverseForceMultiplier;
                mob.AddForce(moveForce);
            }

            wasMoving = !IsUnderMinVelocity();
        }

        private bool IsUnderMinVelocity()
        {
            return Mathf.Abs(mob.GetVelocity().x) < minVelocity;
        }
    }
}