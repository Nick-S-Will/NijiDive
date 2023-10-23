using UnityEngine;

namespace NijiDive.Controls.Movement
{
    public abstract class LinearAnalogMoving : LinearDigitalMoving
    {
        protected override void Move(float input)
        {
            var localMoveAxis = LocalMoveAxis;
            var xVelocity = mob.Velocity.x;

            if (input == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopMove?.Invoke();
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartMove?.Invoke();

                var moveForce = input * moveSpeed * localMoveAxis;
                mob.AddForce(moveForce);
            }

            wasMoving = !IsUnderMinVelocity();
        }
    }
}