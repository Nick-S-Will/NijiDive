using UnityEngine;

using NijiDive.Entities;
using NijiDive.Controls.Attacks;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Product")]
    public class Product : MenuItem
    {
        [SerializeField] [Min(1)] private int baseCost = 100;
        [Space]
        [SerializeField] private BuffType type = BuffType.Health;
        [SerializeField] [Min(1)] private int buffAmount = 1;

        public int Cost => baseCost;
        public BuffType BuffType => type;
        public int BuffAmount => buffAmount;

        public void UseOn(Mob mob)
        {
            switch (type)
            {
                case BuffType.Health:
                    mob.Health.ReceiveHealth(buffAmount);
                    break;
                case BuffType.MaxHealth:
                    mob.Health.ReceiveHealth(buffAmount * mob.Health.MaxHealth);
                    break;
                case BuffType.Ammo:
                    mob.GetControlType<WeaponController>().AddBonusAmmo(buffAmount);
                    break;
            }
        }
    }

    public enum BuffType { Health = 0, MaxHealth = 1, Ammo = 2 }
}