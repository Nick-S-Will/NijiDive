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
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private SpriteRenderer productSelectorRenderer;
        [SerializeField] private SpriteRenderer[] productSpriteRenderers = new SpriteRenderer[Shop.FOR_SALE_COUNT];

        private PlayerController player;
        private Shop shop;
        private int selectedIndex;

        public int SelectedIndex => selectedIndex;
        public bool IsVisible => shopPanel.activeSelf;

        private void Awake()
        {
            Shop.OnShopSpawn.AddListener(SetShop);
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found in scene", this);
                return;
            }

            OnOpen.AddListener(UpdateProductSprites);
            OnOpen.AddListener(DisablePlayerWalking);
            OnClose.AddListener(EnablePlayerWalking);

            GivePlayerShopControl();
        }

        private void SetShop(Shop shop) => this.shop = shop;

        #region Shop Controls
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
            if (player == null) return; // No error since player may have been destroyed

            var shopControl = player.RemoveControlType<ShopControl>();
            shopControl.OnSelect.RemoveListener(Select);
            shopControl.OnCancel.RemoveListener(Exit);
            shopControl.OnLeft.RemoveListener(NavigateLeft);
            shopControl.OnRight.RemoveListener(NavigateRight);
        }

        private void SetPlayerWalking(bool enabled) => player.GetControlType<LocalRightAnalogMoving>().enabled = enabled;
        private void EnablePlayerWalking() => SetPlayerWalking(true);
        private void DisablePlayerWalking() => SetPlayerWalking(false);
        #endregion

        private void UpdateProductSprites()
        {
            for (int i = 0; i < productSpriteRenderers.Length; i++)
            {
                var product = shop.GetProduct(i);
                productSpriteRenderers[i].sprite = product ? shop.GetProduct(i).UISprite : null;
            }
            productSelectorRenderer.transform.position = productSpriteRenderers[selectedIndex].transform.position;
        }

        private void SetVisible(bool visible)
        {
            if (IsVisible == visible) return;

            if (visible) OnOpen?.Invoke();
            else OnClose.Invoke();

            shopPanel.SetActive(visible);
            productSelectorRenderer.gameObject.SetActive(visible);
            foreach (var sr in productSpriteRenderers) sr.gameObject.SetActive(visible);
        }

        #region UI Control
        public void Select()
        {
            if (!IsVisible)
            {
                SetVisible(true);
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
            if (gameObject.activeSelf) SetVisible(false);
        }

        private void Navigate(int indexDirection)
        {
            selectedIndex = (selectedIndex + indexDirection + Shop.FOR_SALE_COUNT) % Shop.FOR_SALE_COUNT;
            productSelectorRenderer.transform.position = productSpriteRenderers[selectedIndex].transform.position;

            OnNavigate?.Invoke();
        }
        public void NavigateLeft() => Navigate(-1);
        public void NavigateRight() => Navigate(1);
        #endregion

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