using UnityEngine;

using NijiDive.Managers.PlayerBased;

namespace NijiDive.MenuItems.Upgrades
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrades/Food")]
    public class Food : Upgrade
    {
        [Space]
        [SerializeField] [Min(1)] private int healAmount = 1;

        public override void Equip()
        {
            PlayerBasedManager.Player.Health.ReceiveHealth(healAmount);
        }
    }
}