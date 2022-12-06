using UnityEngine;

namespace NijiDive.Items
{
    public abstract class Upgrade : Item
    {
        public Sprite MenuSprite => sprite;
        public Sprite UISprite => uiSprite;

        public virtual void Equip()
        {
            // TODO: Add upgrade slots to main ui for uiSprite to be added to
        }
    }
}