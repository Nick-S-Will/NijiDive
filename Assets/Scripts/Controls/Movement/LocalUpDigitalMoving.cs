using System;
using UnityEngine;

namespace NijiDive.Controls.Movement
{
    [Serializable]
    public class LocalUpDigitalMoving : LinearDigitalMoving
    {
        protected override Vector2 MoveAxis => Vector2.up;

        public override void TryToUse()
        {
            Move(mob.LastInputs.lStick.y);
        }
    }
}