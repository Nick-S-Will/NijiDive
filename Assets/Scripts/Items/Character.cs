using UnityEngine;

using NijiDive.Managers.UI;
using NijiDive.Controls;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Character")]
    public class Character : Upgrade
    {
        // private Control[] bonusControls;

        public override void Equip()
        {
            base.Equip();

            // foreach (var control in bonusControls) UIManager.singleton.Player.AddControlType(control);
        }
    }
}