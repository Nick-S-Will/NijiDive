using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class PlayerHealthData : HealthData
    {
        [SerializeField] [Min(1)] private int maxBonusHealth = 4;
        [SerializeField] [Min(0f)] private float hitInterval = 2f;

        public override int MaxHealth { get; protected set; }
        public int MaxBonusHealth => maxBonusHealth;
        public int BonusHealth { get; private set; }

        private float lastDamageTime;

        public override void Reset()
        {
            base.Reset();
            MaxHealth = baseMaxHealth;
            BonusHealth = 0;
            lastDamageTime = -hitInterval;
        }

        public override void ReceiveHealth(int amount)
        {
            if (amount < 1) return;

            Health += amount;
            int surplus = Health > MaxHealth ? Health - MaxHealth : 0;

            BonusHealth += surplus;
            int healthUp = BonusHealth / maxBonusHealth;

            MaxHealth += healthUp;

            Health = Mathf.Min(Health, MaxHealth);
            BonusHealth %= maxBonusHealth;

            OnChangeHealth?.Invoke();
        }

        public override void LoseHealth(int amount)
        {
            if (amount < 1 || Time.time < lastDamageTime + hitInterval) return;

            Health = Mathf.Max(0, Health - amount);
            lastDamageTime = Time.time;

            OnChangeHealth?.Invoke();
        }
    }
}