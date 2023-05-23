using UnityEngine;

namespace NijiDive.MenuItems
{
    public abstract class MenuItem : ScriptableObject
    {
        [SerializeField] private string description;
        [SerializeField] protected Sprite worldSprite;
        [SerializeField] protected Sprite menuUISprite;

        public string Description => description;
        public Sprite WorldSprite => worldSprite;
        public Sprite MenuUISprite => menuUISprite;
    }
}