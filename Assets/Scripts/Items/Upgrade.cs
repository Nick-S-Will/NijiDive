using UnityEngine;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrade")]
    public class Upgrade : MenuItem
    {
        [SerializeField] private Sprite hudSprite;

        public Sprite HUDSprite => hudSprite;

        public virtual void Equip()
        {
            // TODO: Add upgrade slots to main ui for uiSprite to be added to
        }
    }
}