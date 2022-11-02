using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class HealthData
    {
        [SerializeField] [Min(1)] private int baseMaxHealth = 4, maxBonusHealth = 4;
        [Space]
        public UnityEvent OnChangeHealth;

        public int MaxHealth { get; private set; }
        public int MaxBonusHealth => maxBonusHealth;
        public int Health { get; private set; }
        public int BonusHealth { get; private set; }

        public void Reset()
        {
            Health = baseMaxHealth;
            MaxHealth = baseMaxHealth;
            BonusHealth = 0;
        }

        public void ReceiveHealth(int amount)
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

        public void LoseHealth(int amount)
        {
            if (amount < 1) return;

            Health -= amount;

            OnChangeHealth?.Invoke();
        }
    }
}