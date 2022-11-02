using System;
using UnityEngine;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public abstract class Attacking : Control
    {
        [SerializeField] protected LayerMask damageLayers;

        protected Collider2D CheckBounds(Bounds bounds) => Physics2D.OverlapBox(bounds.center, bounds.size, 0f, damageLayers);

        public static bool TryDamageCollider(GameObject sourceObject, Collider2D collider, DamageType damageType, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null) return damageable.TryDamage(sourceObject, int.MaxValue, damageType, collider.ClosestPoint(point));

            return false;
        }
    }
}