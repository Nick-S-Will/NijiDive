using UnityEngine;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Product")]
    public class Product : MenuItem
    {
        [SerializeField] [Min(1)] private int baseCost = 100;
        [Space]
        [SerializeField] private BuffType type = BuffType.Health;
        [SerializeField] [Min(1)] private int buffAmount = 1;

        public Sprite ShopSprite => sprite;
        public Sprite UISprite => uiSprite;
        public int Cost => baseCost;
        public BuffType BuffType => type;
        public int BuffAmount => buffAmount;
    }

    public enum BuffType { Health = 0, Ammo = 1 }
}