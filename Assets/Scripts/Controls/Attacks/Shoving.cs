using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Shoving : Attacking
    {
        public override void Use()
        {
            TryShove();
        }

        private void TryShove()
        {
            if (!mob.LastWallCheck)
            {
                var collider = CheckBounds(mob.WallCheckBounds);
                if (collider && TryDamageCollider(mob, collider, damageType, damage, mob.CeilingCheckBounds.center))
                {
                    OnDamage?.Invoke();
                    if (IsDead(collider)) OnKill?.Invoke();
                }
            }
        }
    }
}