using UnityEngine;

using NijiDive.Managers.PlayerBased;
using NijiDive.Controls.Attacks;
using NijiDive.Controls.Attacks.Specials;

namespace NijiDive.MenuItems.Upgrades
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrades/Blast Module")]
    public class BlastModule : Upgrade
    {
        [Space]
        [SerializeField] private Blast blast;

        public override void Equip()
        {
            blast.Setup(PlayerBasedManager.Player);

            var stomping = PlayerBasedManager.Player.GetControlType<Stomping>();
            stomping.OnDamage.AddListener(blast.TryToSpecial);
            stomping.OnReset.AddListener(() => stomping.OnDamage.RemoveListener(blast.TryToSpecial));
        }
    }
}