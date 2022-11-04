using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Health
{
    [Serializable]
    public class HealthData
    {
        [SerializeField] [Min(1)] protected int baseMaxHealth = 1;
        [Space]
        public UnityEvent OnChangeHealth;

        public int Health { get; protected set; }
        public bool IsEmpty => Health == 0;

        public virtual void Reset()
        {
            Health = baseMaxHealth;
        }

        private void ChangeHealth(int amount)
        {
            Health = Mathf.Clamp(Health + amount, 0, baseMaxHealth);

            OnChangeHealth?.Invoke();
        }

        public virtual void ReceiveHealth(int amount)
        {
            if (amount < 1) return;
            ChangeHealth(amount);
        }

        public virtual void LoseHealth(int amount)
        {
            if (amount < 1) return;
            ChangeHealth(-amount);
        }
    }
}