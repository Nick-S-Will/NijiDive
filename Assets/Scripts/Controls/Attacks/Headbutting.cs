using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Headbutting : Attacking
    {
        public override void FixedUpdate()
        {
            TryHeadbutt();
        }

        private void TryHeadbutt()
        {
            if (!mob.LastGroundCheck)
            {
                var collider = CheckBounds(mob.CeilingCheckBounds);
                if (collider && TryDamageCollider(mob.gameObject, collider, damageType, damage, mob.CeilingCheckBounds.center))
                {
                    OnDamage?.Invoke();
                    if (IsDead(collider)) OnKill?.Invoke();
                    mob.SetVelocityY(0f);
                }
            }
        }
    }
}