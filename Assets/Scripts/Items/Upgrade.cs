using UnityEngine;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Upgrade")]
    public class Upgrade : MenuItem
    {
        [SerializeField] private Sprite hUDSprite;

        public Sprite HUDSprite => hUDSprite;

        public virtual void Equip()
        {

        }
    }
}