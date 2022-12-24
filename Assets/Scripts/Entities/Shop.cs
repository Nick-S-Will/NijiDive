using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Coins;
using NijiDive.Entities.Mobs.Player;
using NijiDive.MenuItems;
using NijiDive.Utilities;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class Shop : Entity
    {
        [Space]
        // bool is if the contact started or ended
        public UnityEvent<bool> OnPlayerContact;

        [SerializeField] private List<Product> productOptions;
        [SerializeField] private SpriteRenderer[] productSlotRenderers = new SpriteRenderer[FOR_SALE_COUNT];
        [Space]
        [SerializeField] private BoxCollider2D purchaseBounds;

        private PlayerController player;
        private readonly Product[] productsForSale = new Product[FOR_SALE_COUNT];
        private readonly bool[] inStock = new bool[FOR_SALE_COUNT];

        // Shop is the shop that spawned
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
                var sprite = inStock[i] ? productsForSale[i].WorldSprite : null;
                productSlotRenderers[i].sprite = sprite;
            }
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

        #region Product Access
        /// <summary>
        /// Gets access to <see cref="Product"/> at <paramref name="selectedIndex"/> if in range and in stock
        /// </summary>
        public Product GetProduct(int selectedIndex)
        {
            if (!IndexIsInRange(selectedIndex)) return null;

            return inStock[selectedIndex] ? productsForSale[selectedIndex] : null;
        }

        public Product[] GetProducts()
        {
            var products = new Product[productsForSale.Length];

            for (int i = 0; i < productsForSale.Length; i++) products[i] = GetProduct(i);
            return products;
        }

        /// <summary>
        /// Purchases <see cref="Product"/> at <paramref name="selectedIndex"/> and removes it from stock if in range and in stock
        /// </summary>
        private Product TakeProduct(int selectedIndex)
        {
            if (!IndexIsInRange(selectedIndex)) return null;

            if (inStock[selectedIndex])
            {
                productSlotRenderers[selectedIndex].sprite = null;
                inStock[selectedIndex] = false;
                return productsForSale[selectedIndex];
            }
            else return null;
        }

        public int TryPurchase(int index)
        {
            var product = GetProduct(index);
            if (product == null) return 0;

            if (CoinManager.CoinCount >= product.Cost)
            {
                CoinManager.singleton.UseCoins(product.Cost);
                _ = TakeProduct(index);

                product.UseOn(player);

                return product.Cost;
            }

            return 0;
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (player) return;

            player = collider.GetComponent<PlayerController>();
            if (player)
            {
                if (collider.IsTouching(purchaseBounds)) OnPlayerContact?.Invoke(true);
                else player = null;
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (player == null || !player.HasBaseFeaturesEnabled()) return;

            if (player == collider.GetComponent<PlayerController>())
            {
                OnPlayerContact?.Invoke(false);
                player = null;
            }
        }

        private void OnValidate()
        {
            if (productSlotRenderers.Length != FOR_SALE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(productSlotRenderers)} must be the same as {nameof(FOR_SALE_COUNT)}");
                Array.Resize(ref productSlotRenderers, FOR_SALE_COUNT);
            }
        }
    }
}