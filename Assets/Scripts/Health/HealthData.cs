using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class HealthData
    {
        public UnityEvent OnChangeHealth;
        [Space]
        [SerializeField] [Min(0f)] private float hitInterval = 0f;
        [Space]
        [SerializeField] [Min(1)] protected int baseMaxHealth = 1;

        public float BonusHitInterval { get; set; }
        public int HealthPoints { get; protected set; }
        public virtual int MaxHealthPoints { get => baseMaxHealth; protected set => baseMaxHealth = value; }
        public bool IsEmpty => HealthPoints == 0;

        private float lastDamageTime;

        public virtual void Reset()
        {
            HealthPoints = baseMaxHealth;
            lastDamageTime = -hitInterval;
            BonusHitInterval = 0;
        }

        public bool CanLoseHealth() => Time.time >= lastDamageTime + hitInterval + BonusHitInterval;

        private void ChangeHealth(int amount)
        {
            HealthPoints = Mathf.Clamp(HealthPoints + amount, 0, baseMaxHealth);

            OnChangeHealth?.Invoke();
        }

        public virtual void ReceiveHealth(int amount)
        {
            if (amount < 1) return;
            ChangeHealth(amount);
        }

        public virtual void LoseHealth(int amount)
        {
            if (amount < 1 || !CanLoseHealth()) return;

            ChangeHealth(-amount);
            lastDamageTime = Time.time;
        }
    }
}