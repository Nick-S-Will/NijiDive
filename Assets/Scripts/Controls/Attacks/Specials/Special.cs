using System;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public abstract class Special : Attacking
    {
        [SerializeField] private UnityEvent OnUse, OnEmpty;

        [SerializeField] private int maxCharges = 4;

        protected int charges = int.MaxValue; // Max for testing

        public sealed override void TryToUse()
        {
            if (mob.LastInputs.altDownThisFrame && charges > 0)
            {
                Use();
                OnUse.Invoke();

                charges--;
            }
            else OnEmpty?.Invoke();
        }

        protected abstract void Use();

        public override void Reset()
        {
            charges = 0;
        }
    }
}