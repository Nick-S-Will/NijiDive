using System;
using UnityEngine;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Shoving : Attacking
    {
        public override void TryToUse()
        {
            if (!mob.LastWallCheck)
            {
                var collider = CheckBounds(mob.WallCheckBounds);
                if (collider) _ = TryDamageCollider(mob, collider, damageType, damage, mob.CeilingCheckBounds.center);
            }
        }
    }
}