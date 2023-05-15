using UnityEngine;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrade")]
    public class Upgrade : MenuItem
    {
        public virtual void Equip() { }
    }
}