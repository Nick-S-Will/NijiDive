using UnityEngine;

namespace NijiDive.Items
{
    [CreateAssetMenu(menuName = "NijiDive/Items/Product")]
    public class Product : ScriptableObject
    {
        [SerializeField] private Sprite shopSprite, uiSprite;
        [SerializeField] [Min(1)] private int baseCost = 100;
        [Space]
        [SerializeField] private ProductType type = ProductType.Health;
        [SerializeField] [Min(1)] private int buffAmount = 1;

        public Sprite ShopSprite => shopSprite;
        public Sprite UISprite => uiSprite;
        public int Cost => baseCost;
        public ProductType BuffType => type;
        public int BuffAmount => buffAmount;
    }

    public enum ProductType { Health = 0, Ammo = 1 }
}