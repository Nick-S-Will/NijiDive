using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class PlayerHealthData : HealthData
    {
        [SerializeField] [Min(1)] private int maxBonusHealth = 4;

        public int MaxHealth { get; private set; }
        public int MaxBonusHealth => maxBonusHealth;
        public int BonusHealth { get; private set; }

        public override void Reset()
        {
            base.Reset();
            MaxHealth = baseMaxHealth;
            BonusHealth = 0;
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
            BonusHealth = BonusHealth % maxBonusHealth;

            OnChangeHealth?.Invoke();
        }

        public override void LoseHealth(int amount)
        {
            if (amount < 1) return;

            Health = Mathf.Max(0, Health - amount);

            OnChangeHealth?.Invoke();
        }
    }
}