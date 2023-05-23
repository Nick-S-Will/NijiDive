using UnityEngine;

using NijiDive.Managers.PlayerBased;

namespace NijiDive.MenuItems.Upgrades
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrades/Candle")]
    public class Candle : Upgrade
    {
        [Space]
        [SerializeField] [Min(0f)] private float extraInvincibilityDuration = 1f;

        public override void Equip()
        {
            PlayerBasedManager.Player.Health.BonusHitInterval += extraInvincibilityDuration;
        }
    }
}