using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Shoving : Attacking
    {
        [Space]
        public UnityEvent OnShove;

        public override void FixedUpdate()
        {
            TryShove();
        }

        private void TryShove()
        {
            if (!mob.LastWallCheck)
            {
                var collider = CheckBounds(mob.WallCheckBounds);
                if (collider && TryDamageCollider(mob.gameObject, collider, damageType, damage, mob.CeilingCheckBounds.center))
                {
                    OnShove?.Invoke();
                }
            }
        }
    }
}