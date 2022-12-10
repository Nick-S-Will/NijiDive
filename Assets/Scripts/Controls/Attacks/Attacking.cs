using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Entities;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public abstract class Attacking : Control
    {
        public UnityEvent OnDamage, OnKill;
        [SerializeField] protected LayerMask damageLayers;
        [SerializeField] protected DamageType damageType = DamageType.Enemy;
        [SerializeField] protected int damage = 1;

        protected Collider2D CheckBounds(Bounds bounds) => Physics2D.OverlapBox(bounds.center, bounds.size, 0f, damageLayers);

        public static bool TryDamageCollider(MonoBehaviour sourceBehaviour, Collider2D collider, DamageType damageType, int damage, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null) return damageable.TryDamage(sourceBehaviour, damage, damageType, collider.ClosestPoint(point));

            return false;
        }

        public static bool IsDead(Collider2D collider)
        {
            var mob = collider.GetComponent<Mob>();
            if (mob) return mob.Health.IsEmpty;

            return false;
        }
    }
}