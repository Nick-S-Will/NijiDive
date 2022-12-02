using UnityEngine;

namespace NijiDive.Items
{
    public abstract class Upgrade : ScriptableObject
    {
        [SerializeField] private Sprite menuSprite, uiSprite;
        [SerializeField] private string description;

        public Sprite MenuSprite => menuSprite;
        public Sprite UISprite => uiSprite;
        public string Description => description;

        public virtual void Equip()
        {
            // TODO: Add upgrade slots to main ui for uiSprite to be added to
        }
    }
}