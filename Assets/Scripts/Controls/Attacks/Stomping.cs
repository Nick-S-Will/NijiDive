using System;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public class Stomping : Attacking
    {
        public UnityEvent OnStomp;

        public override void FixedUpdate()
        {
            TryStomp();
        }

        private void TryStomp()
        {
            var collider = CheckBounds(mob.GroundCheckBounds);
            if (collider && TryDamage(collider, DamageType.Player | DamageType.Stomp, mob.transform.position))
            {
                OnStomp?.Invoke();
            }
        }
    }
}