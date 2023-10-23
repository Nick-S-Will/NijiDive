using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Movement
{
    public abstract class LinearDigitalMoving : Moving
    {
        public UnityEvent OnStartMove, OnStopMove;
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] [Min(0f)] protected float minVelocity = 0.1f;

        protected bool wasMoving;

        protected abstract Vector2 MoveAxis { get; }
        protected Vector2 LocalMoveAxis => Quaternion.AngleAxis(mob.transform.rotation.z, Vector3.forward) * MoveAxis;

        /// <summary>
        /// Updates <see cref="Mob.Rb2d"/>'s velocity on <see cref="moveAxis"/>
        /// </summary>
        /// <param name="input">Scales the applied movement force</param>
        protected virtual void Move(float input)
        {
            var localMoveAxis = LocalMoveAxis;

            mob.Velocity -= (Vector2)Vector3.Project(mob.Velocity, localMoveAxis);
            if (input == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopMove?.Invoke();
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartMove?.Invoke();

                mob.Velocity += (Vector2)(input * moveSpeed * localMoveAxis);
            }

            wasMoving = !IsUnderMinVelocity();
        }

        protected bool IsUnderMinVelocity()
        {
            return Mathf.Abs(mob.Velocity.x) < minVelocity;
        }
    }
}