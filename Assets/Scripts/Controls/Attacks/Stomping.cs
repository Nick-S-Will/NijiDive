using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Stomping : Attacking
    {
        [Space]
        public UnityEvent OnStomp;

        public override void FixedUpdate()
        {
            TryStomp();
        }

        private void TryStomp()
        {
            var collider = CheckBounds(mob.GroundCheckBounds);
            if (collider && TryDamageCollider(mob.gameObject, collider, damageType, damage, mob.transform.position))
            {
                OnStomp?.Invoke();
            }
        }
    }
}