using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Headbutting : Attacking
    {
        public override void TryToUse()
        {
            if (!mob.LastGroundCheck)
            {
                var collider = CheckBounds(mob.CeilingCheckBounds);
                if (collider && TryDamageCollider(mob, collider, damageType, damage, mob.CeilingCheckBounds.center)) mob.SetVelocityY(0f);
            }
        }
    }
}