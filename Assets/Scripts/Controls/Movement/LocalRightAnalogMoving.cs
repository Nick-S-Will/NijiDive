using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class LocalRightAnalogMoving : LocalRightDigitalMoving
    {
        [Tooltip("The force multiplier added when starting to move the opposite way")]
        [SerializeField] [Min(1f)] private float reverseForceMultiplier = 1f;
        [SerializeField] [Min(0f)] private float extraDeceleration = 0f;

        public override void FixedUpdate()
        {
            Move(mob.LastInputs.lStick.x);
        }

        /// <summary>
        /// Updates <see cref="Rb2d"/>'s X axis velocity
        /// </summary>
        /// <param name="xInput">Scales the applied movement force</param>
        protected override void Move(float xInput)
        {
            var xVelocity = mob.velocity.x;
            if (xInput == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopWalk?.Invoke();
                
                mob.AddForce(-extraDeceleration * xVelocity * mob.transform.right);
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartWalk?.Invoke();

                var moveForce = xInput * moveSpeed * mob.transform.right;
                if (xInput * xVelocity < 0f) moveForce *= reverseForceMultiplier;
                mob.AddForce(moveForce);
            }

            wasMoving = !IsUnderMinVelocity();
        }
    }
}