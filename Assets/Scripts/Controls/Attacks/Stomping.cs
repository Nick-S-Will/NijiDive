using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Stomping : Attacking
    {

        public override void Use()
        {
            TryStomp();
        }

        private void TryStomp()
        {
            var collider = CheckBounds(mob.GroundCheckBounds);
            if (collider && TryDamageCollider(mob.gameObject, collider, damageType, damage, mob.transform.position))
            {
                OnDamage?.Invoke();
                if (IsDead(collider)) OnKill?.Invoke();
            }
        }
    }
}