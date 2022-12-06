using System;
using UnityEngine;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class LocalRightAnalogMoving : LinearAnalogMoving
    {
        protected override Vector2 MoveAxis => Vector2.right;

        public override void Use()
        {
            Move(mob.LastInputs.lStick.x);
        }
    }
}