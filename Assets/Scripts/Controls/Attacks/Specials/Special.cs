using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.PlayerBased.Combo;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public abstract class Special : Attacking
    {
        public UnityEvent OnCharge, OnUse, OnEmpty;

        [SerializeField] [Min(1)] private int comboForCharge = 5, maxCharges = 4;
        [SerializeField] private bool infiniteCharges;

        private int charges, comboCountOnLastCharge;

        public int Charges => charges;
        public int MaxCharges => maxCharges;

        public override void Start()
        {
            ComboManager.OnCombo.AddListener(TryCharge);
            ComboManager.OnEndCombo.AddListener(ResetComboCountOnLastCharge);
        }

        private void TryCharge(int comboCount)
        {
            var addedCharge = (comboCount - comboCountOnLastCharge) / comboForCharge;

            if (addedCharge > 0)
            {
                charges = Mathf.Min(charges + addedCharge, maxCharges);
                comboCountOnLastCharge = comboCount;
                OnCharge.Invoke();
            }
        }

        private void ResetComboCountOnLastCharge(int _) => comboCountOnLastCharge = 0;

        public sealed override void TryToUse()
        {
            if (mob.LastInputs.altDownThisFrame && (charges > 0 || infiniteCharges))
            {
                Use();
                OnUse.Invoke();

                charges = Mathf.Max(0, charges - 1);
            }
            else OnEmpty?.Invoke();
        }

        protected abstract void Use();

        public override void Reset()
        {
            charges = 0;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            ComboManager.OnCombo.RemoveListener(TryCharge);
        }
    }
}