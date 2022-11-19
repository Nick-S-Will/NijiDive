using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Coins;
using NijiDive.Controls.Player;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;
using NijiDive.Items;
using NijiDive.Utilities;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class Shop : Entity
    {
        [SerializeField] private List<Product> productOptions;
        [SerializeField] private SpriteRenderer[] slots = new SpriteRenderer[FOR_SALE_COUNT];
        [Space]
        [SerializeField] private BoxCollider2D purchaseBounds;

        private PlayerController player;
        private Product[] productsForSale = new Product[FOR_SALE_COUNT];
        private bool[] inStock = new bool[FOR_SALE_COUNT];

        public static UnityEvent<Shop> OnShopSpawn = new UnityEvent<Shop>();
        public const int FOR_SALE_COUNT = 3;

        private void Start()
        {
            AssignProductsForSale();
            DisplayProductsForSale();

            OnShopSpawn?.Invoke(this);
        }

        private void AssignProductsForSale()
        {
            var shuffledProducts = productOptions.ToArray();
            shuffledProducts.Shuffle();

            for (int i = 0; i < productsForSale.Length; i++)
            {
                productsForSale[i] = shuffledProducts[i];
                inStock[i] = true;
            }
        }

        private void DisplayProductsForSale()
        {
            for (int i = 0; i < productsForSale.Length; i++)
            {
                var sprite = inStock[i] ? productsForSale[i].ShopSprite : null;
                slots[i].sprite = sprite;
            }
        }

        private void SetShopControls(bool enabled)
        {
            if (player == null) return;

            player.GetControlType<Jumping>().enabled = !enabled;
            player.GetControlType<ShopControl>().enabled = enabled;
        }

        private bool IndexIsInRange(int index)
        {
            if (index < 0 || index >= FOR_SALE_COUNT)
            {
                Debug.LogError($"Given index {index} is out of range [0, {FOR_SALE_COUNT - 1}]", this);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets you access to <see cref="Product"/> at <paramref name="selectedIndex"/> if in range and in stock
        /// </summary>
        public Product GetProduct(int selectedIndex)
        {
            if (!IndexIsInRange(selectedIndex)) return null;

            return inStock[selectedIndex] ? productsForSale[selectedIndex] : null;
        }
        /// <summary>
        /// Purchases <see cref="Product"/> at <paramref name="selectedIndex"/> and removes it from stock if in range and in stock
        /// </summary>
        private Product TakeProduct(int selectedIndex)
        {
            if (!IndexIsInRange(selectedIndex)) return null;

            if (inStock[selectedIndex])
            {
                slots[selectedIndex].sprite = null;
                inStock[selectedIndex] = false;
                return productsForSale[selectedIndex];
            }
            else return null;
        }

        public int TryPurchaseSelection(int index)
        {
            var product = GetProduct(index);
            if (product == null) return 0;

            if (CoinManager.singleton.CoinCount >= product.Cost)
            {
                CoinManager.singleton.UseCoins(product.Cost);
                _ = TakeProduct(index);

                switch (product.BuffType)
                {
                    case BuffType.Health:
                        player.Health.ReceiveHealth(product.BuffAmount);
                        break;
                    case BuffType.Ammo:
                        player.GetControlType<WeaponController>().AddBonusAmmo(product.BuffAmount);
                        break;
                }

                return product.Cost;
            }

            return 0;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (player) return; 
            
            player = collider.GetComponent<PlayerController>();
            if (player && collider.IsTouching(purchaseBounds)) SetShopControls(true);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (player == collider.GetComponent<PlayerController>())
            {
                SetShopControls(false);
                player = null;
            }
        }

        private void OnValidate()
        {
            if (slots.Length != FOR_SALE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(slots)} must be the same as {nameof(FOR_SALE_COUNT)}");
                Array.Resize(ref slots, FOR_SALE_COUNT);
            }
        }
    }
}