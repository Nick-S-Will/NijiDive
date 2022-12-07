using UnityEngine;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrade")]
    public class Upgrade : MenuItem
    {
        public Sprite MenuSprite => sprite;
        public Sprite HUDSprite => uiSprite;

        public virtual void Equip()
        {
            // TODO: Add upgrade slots to main ui for uiSprite to be added to
        }
    }
}