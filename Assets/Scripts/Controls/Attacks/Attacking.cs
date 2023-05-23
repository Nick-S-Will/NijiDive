using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Entities.Mobs;

namespace NijiDive.Controls.Attacks
{
    [Serializable]
    public abstract class Attacking : Control
    {
        public UnityEvent OnDamage, OnKill;
        [SerializeField] protected LayerMask damageLayers;
        [SerializeField] protected DamageType damageType = DamageType.Enemy;
        [SerializeField] [Min(0)] protected int damage = 1;

        protected Collider2D CheckBounds(Bounds bounds) => Physics2D.OverlapBox(bounds.center, bounds.size, 0f, damageLayers);
        protected (Collider2D, Vector2)[] SampleBounds(Bounds bounds, int xSamples, int ySamples)
        {
            var sampleSize = new Vector2(bounds.size.x / xSamples, bounds.size.y / ySamples);
            var sampleList = new List<(Collider2D, Vector2)>();

            for (int y = 0; y < ySamples; y++)
            {
                for (int x = 0; x < xSamples; x++)
                {
                    var samplePosition = (Vector2)bounds.min + new Vector2(sampleSize.x * (x + 0.5f), sampleSize.y * (y + 0.5f));
                    var collider = Physics2D.OverlapBox(samplePosition, sampleSize, 0f, damageLayers);
                    if (collider) sampleList.Add((collider, samplePosition));
                }
            }

            return sampleList.ToArray();
        }
        protected (Collider2D, Vector2)[] SampleBounds(Bounds bounds) => SampleBounds(bounds, Mathf.CeilToInt(bounds.size.x), Mathf.CeilToInt(bounds.size.y));

        public bool TryDamageCollider(MonoBehaviour sourceBehaviour, Collider2D collider, DamageType damageType, int damage, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.TryDamage(sourceBehaviour, damage, damageType, point))
            {
                InvokeDamageEvents(collider);
                return true;
            }

            return false;
        }
        public bool TryDamageSamples(MonoBehaviour sourceBehaviour, (Collider2D, Vector2)[] samples, DamageType damageType, int damage)
        {
            bool didDamage = false;
            foreach (var sample in samples)
            {
                if (TryDamageCollider(sourceBehaviour, sample.Item1, damageType, damage, sample.Item2)) didDamage = true;
            }

            return didDamage;
        }

        public static bool IsDead(Collider2D collider)
        {
            var mob = collider.GetComponent<Mob>();
            if (mob) return mob.Health.IsEmpty;

            return false;
        }

        public void InvokeDamageEvents(Collider2D targetCollider)
        {
            OnDamage?.Invoke();
            if (IsDead(targetCollider)) OnKill?.Invoke();
        }
    }
}