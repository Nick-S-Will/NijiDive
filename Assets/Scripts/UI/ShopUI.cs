using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Coins;
using NijiDive.Controls.Player;
using NijiDive.Controls.Movement;
using NijiDive.Entities;
using NijiDive.Items;
using NijiDive.Health;

namespace NijiDive.UI
{
    public class ShopUI : MonoBehaviour
    {
        public UnityEvent OnOpen, OnClose, OnNavigate, OnBroke;
        public UnityEvent<int> OnPurchase;
        [Space]
        [SerializeField] private SpriteRenderer[] productSpriteRenderers = new SpriteRenderer[Shop.FOR_SALE_COUNT];
        [SerializeField] private SpriteRenderer productSelector;

        private PlayerController player;
        private Shop shop;

        private int selectedIndex;

        public int SelectedIndex => selectedIndex;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found in scene", this);
                return;
            }

            shop = FindObjectOfType<Shop>();
            if (shop == null)
            {
                Debug.LogError($"No {nameof(Shop)} found in scene", this);
                return;
            }

            OnOpen.AddListener(UpdateProductSprites);
            OnOpen.AddListener(EnableShopControls);
            OnClose.AddListener(DisableShopControls);
            
            GivePlayerShopControl();

            gameObject.SetActive(false);
        }

        private void GivePlayerShopControl()
        {
            var shopControl = new ShopControl();
            shopControl.OnSelect.AddListener(Select);
            shopControl.OnCancel.AddListener(Exit);
            shopControl.OnLeft.AddListener(NavigateLeft);
            shopControl.OnRight.AddListener(NavigateRight);
            shopControl.mob = player;
            shopControl.enabled = false;

            player.AddControlType(shopControl);
        }
        private void RemovePlayerShopControl()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null) return; // No error since player may have been destroyed

            var shopControl = player.RemoveControlType<ShopControl>();
            shopControl.OnSelect.RemoveListener(Select);
            shopControl.OnCancel.RemoveListener(Exit);
            shopControl.OnLeft.RemoveListener(NavigateLeft);
            shopControl.OnRight.RemoveListener(NavigateRight);
        }

        private void SetShopControls(bool enabled)
        {
            player.GetControlType<LocalRightAnalogMoving>().enabled = !enabled;
            player.GetControlType<ShopControl>().enabled = enabled;
        }
        private void EnableShopControls() => SetShopControls(true);
        private void DisableShopControls() => SetShopControls(false);

        private void UpdateProductSprites()
        {
            for (int i = 0; i < productSpriteRenderers.Length; i++)
            {
                var product = shop.GetProduct(i);
                productSpriteRenderers[i].sprite = product ? shop.GetProduct(i).UISprite : null;
            }
            productSelector.transform.position = productSpriteRenderers[0].transform.position;
        }

        private void SetUIActive(bool enabled)
        {
            if (gameObject.activeSelf == enabled) return;

            if (enabled) OnOpen?.Invoke();
            else OnClose.Invoke();

            gameObject.SetActive(enabled);
        }

        public void Select()
        {
            if (!gameObject.activeSelf)
            {
                SetUIActive(true);
                return;
            }

            int cost = shop.TryPurchaseSelection(selectedIndex);
            if (cost > 0)
            {
                productSpriteRenderers[selectedIndex].sprite = null;
                OnPurchase?.Invoke(cost);
            }
            else OnBroke?.Invoke();
        }

        public void Exit()
        {
            if (gameObject.activeSelf) SetUIActive(false);
        }

        private void Navigate(int indexDirection)
        {
            selectedIndex = (selectedIndex + indexDirection + Shop.FOR_SALE_COUNT) % Shop.FOR_SALE_COUNT;
            productSelector.transform.position = productSpriteRenderers[selectedIndex].transform.position;

            OnNavigate?.Invoke();
        }
        public void NavigateLeft() => Navigate(-1);
        public void NavigateRight() => Navigate(1);

        private void OnValidate()
        {
            if (productSpriteRenderers.Length != Shop.FOR_SALE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(productSpriteRenderers)} must be the same as {nameof(Shop.FOR_SALE_COUNT)}");
                Array.Resize(ref productSpriteRenderers, Shop.FOR_SALE_COUNT);
            }
        }

        private void OnDestroy()
        {
            RemovePlayerShopControl();
        }
    }
}