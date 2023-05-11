using System;
using UnityEngine;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Stomping : Attacking
    {
        public override void TryToUse()
        {
            var collider = CheckBounds(mob.GroundCheckBounds);
            if (collider) TryDamageCollider(mob, collider, damageType, damage, mob.transform.position);
        }
    }
}