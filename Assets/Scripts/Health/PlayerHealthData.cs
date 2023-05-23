using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class PlayerHealthData : HealthData
    {
        [SerializeField] [Min(1)] private int maxBonusHealth = 4;

        public override int MaxHealthPoints { get; protected set; }
        public int MaxBonusHealth => maxBonusHealth;
        public int BonusHealth { get; private set; }

        public override void Reset()
        {
            base.Reset();
            MaxHealthPoints = baseMaxHealth;
            BonusHealth = 0;

            OnChangeHealth?.Invoke();
        }

        public override void ReceiveHealth(int amount)
        {
            if (amount < 1) return;

            HealthPoints += amount;
            int surplus = HealthPoints > MaxHealthPoints ? HealthPoints - MaxHealthPoints : 0;

            BonusHealth += surplus;
            int healthUp = BonusHealth / maxBonusHealth;

            MaxHealthPoints += healthUp;

            HealthPoints = Mathf.Min(HealthPoints, MaxHealthPoints);
            BonusHealth %= maxBonusHealth;

            OnChangeHealth?.Invoke();
        }
    }
}