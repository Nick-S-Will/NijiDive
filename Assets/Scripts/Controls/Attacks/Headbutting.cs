using System;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Headbutting : Attacking
    {
        public UnityEvent OnHeadbutt;

        public override void FixedUpdate()
        {
            TryHeadbutt();
        }

        private void TryHeadbutt()
        {
            if (!mob.lastGroundCheck)
            {
                var collider = CheckBounds(mob.CeilingCheckBounds);
                if (collider && TryDamage(collider, DamageType.Player | DamageType.Headbutt, mob.CeilingCheckBounds.center))
                {
                    OnHeadbutt?.Invoke();
                    mob.SetVelocityY(0f);
                }
            }
        }
    }
}