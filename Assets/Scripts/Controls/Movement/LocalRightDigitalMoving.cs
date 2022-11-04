using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class LocalRightDigitalMoving : Moving
    {
        [SerializeField] protected float moveSpeed = 5f;
        [SerializeField] [Min(0f)] protected float minVelocity = 0.1f;
        [Space]
        public UnityEvent OnStartWalk, OnStopWalk;

        protected bool wasMoving;

        public override void FixedUpdate()
        {
            Move(mob.LastInputs.lStick.x);
        }

        /// <summary>
        /// Updates <see cref="Rb2d"/>'s X axis velocity
        /// </summary>
        /// <param name="xInput">Scales the applied movement force</param>
        protected virtual void Move(float xInput)
        {
            mob.velocity -= (Vector2)Vector3.Project(mob.velocity, mob.transform.right);
            if (xInput == 0f)
            {
                if (wasMoving && IsUnderMinVelocity()) OnStopWalk?.Invoke();
            }
            else
            {
                if (IsUnderMinVelocity()) OnStartWalk?.Invoke();

                mob.velocity += (Vector2)(xInput * moveSpeed * mob.transform.right);
            }

            wasMoving = !IsUnderMinVelocity();
        }

        protected bool IsUnderMinVelocity()
        {
            return Mathf.Abs(mob.velocity.x) < minVelocity;
        }
    }
}