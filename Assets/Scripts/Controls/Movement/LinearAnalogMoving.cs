using UnityEngine;

namespace NijiDive.Controls.Movement
{
    public abstract class LinearAnalogMoving : LinearDigitalMoving
    {
        [Tooltip("The force multiplier added when starting to move the opposite way")]
        [SerializeField] [Min(1f)] protected float reverseForceMultiplier = 1f;
        [SerializeField] [Min(0f)] protected float extraDeceleration = 0f;

        protected override void Move(float input)
        {
            var localMoveAxis = LocalMoveAxis;
            var xVelocity = mob.Velocity.x;

            if (input == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopMove?.Invoke();

                mob.AddForce(-extraDeceleration * xVelocity * localMoveAxis);
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartMove?.Invoke();

                var moveForce = input * moveSpeed * localMoveAxis;
                if (input * xVelocity < 0f) moveForce *= reverseForceMultiplier;
                mob.AddForce(moveForce);
            }

            wasMoving = !IsUnderMinVelocity();
        }
    }
}