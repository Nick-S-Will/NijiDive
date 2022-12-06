using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.UI;
using NijiDive.Entities;
using NijiDive.Controls.Movement;
using NijiDive.Controls.UI;

namespace NijiDive.UI
{
    public class ShopUI : UIMenu
    {
        [Space]
        public UnityEvent<int> OnPurchase;
        public UnityEvent OnBroke;

        private Shop shop;

        private void Awake()
        {
            Shop.OnShopSpawn.AddListener(SetShop);
        }

        private void Start()
        {
            OnOpen.AddListener(UpdateProductSprites);
            OnOpen.AddListener(DisablePlayerWalking);
            OnClose.AddListener(EnablePlayerWalking);
        }

        private void SetShop(Shop shop)
        {
            if (shop) shop.OnPlayerContact.RemoveListener(SetMenuControls);
            this.shop = shop;
            shop.OnPlayerContact.AddListener(SetMenuControls);
        }

        private void UpdateSelectedGraphicsAndText()
        {
            UpdateSelectedGraphics();
            var product = shop.GetProduct(SelectedIndex);
            itemNameText.text = product ? product.name : string.Empty;
            itemDescriptionText.text = product ? product.Description : string.Empty;
        }

        private void UpdateProductSprites()
        {
            for (int i = 0; i < itemSpriteRenderers.Length; i++)
            {
                var product = shop.GetProduct(i);
                itemSpriteRenderers[i].sprite = product ? product.UISprite : null;
            }

            UpdateSelectedGraphicsAndText();
        }

        public override void SetVisible(bool visible)
        {
            if (IsVisible == visible) return;

            if (visible) OnOpen?.Invoke();
            else OnClose.Invoke();

            base.SetVisible(visible);
        }

        #region Setting Shop Controls
        private void SetPlayerWalking(bool enabled) => UIManager.singleton.Player.GetControlType<LocalRightAnalogMoving>().enabled = enabled;
        private void EnablePlayerWalking() => SetPlayerWalking(true);
        private void DisablePlayerWalking() => SetPlayerWalking(false);

        protected override void SetMenuControls(bool enabled)
        {
            if (UIManager.singleton.Player == null) return;

            UIManager.singleton.Player.GetControlType<Jumping>().enabled = !enabled;

            var uiControl = UIManager.singleton.Player.GetControlType<UIControl>();
            uiControl.enabled = enabled;

            SetEventListeners(
                new UnityEvent[] { uiControl.OnSelect, uiControl.OnCancel, uiControl.OnLeft, uiControl.OnRight },
                new UnityAction[] { Select, Exit, NavigateLeft, NavigateRight },
                enabled);
        }
        #endregion

        #region UI Control
        public override void Select()
        {
            if (!IsVisible)
            {
                SetVisible(true);
                return;
            }

            int cost = shop.TryPurchase(SelectedIndex);
            if (cost > 0)
            {
                itemSpriteRenderers[SelectedIndex].sprite = null;
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
            SelectedIndex = (SelectedIndex + indexDirection + Shop.FOR_SALE_COUNT) % Shop.FOR_SALE_COUNT;
            UpdateSelectedGraphicsAndText();

            OnNavigate?.Invoke();
        }
        public override void NavigateLeft() => Navigate(-1);
        public override void NavigateRight() => Navigate(1);
        #endregion

        protected override void OnValidate()
        {
            if (itemSpriteRenderers.Length != Shop.FOR_SALE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(itemSpriteRenderers)} must be the same as {nameof(Shop.FOR_SALE_COUNT)}");
                Array.Resize(ref itemSpriteRenderers, Shop.FOR_SALE_COUNT);
            }

            base.OnValidate();
        }
    }
}