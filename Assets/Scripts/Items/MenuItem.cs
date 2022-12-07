using UnityEngine;

namespace NijiDive.MenuItems
{
    public abstract class MenuItem : ScriptableObject
    {
        [SerializeField] protected Sprite sprite, uiSprite;
        [SerializeField] private string description;

        public string Description => description;
    }
}