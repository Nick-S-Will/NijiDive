using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.UI;
using NijiDive.Entities;
using NijiDive.Controls.Movement;
using NijiDive.Controls.UI;

namespace NijiDive.UI
{
    public class ShopUI : UIMenuItemStore
    {
        private Shop shop;

        private void Awake()
        {
            Shop.OnShopSpawn.AddListener(SetShop);
        }

        protected override void Start()
        {
            base.Start();
            OnOpen.AddListener(() => UpdateMenuItemSprites(shop.GetProducts()));
        }

        private void SetShop(Shop shop)
        {
            if (shop) shop.OnPlayerContact.RemoveListener(SetMenuControls);
            this.shop = shop;
            shop.OnPlayerContact.AddListener(SetMenuControls);
        }

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
            UpdateSelectedGraphicsAndText(shop.GetProduct(SelectedIndex));

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