using System;
using UnityEngine;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class LocalRightDigitalMoving : LinearDigitalMoving
    {
        protected override Vector2 MoveAxis => Vector2.right;

        public override void FixedUpdate()
        {
            Move(mob.LastInputs.lStick.x);
        }
    }
}