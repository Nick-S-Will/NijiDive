using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.UI;
using NijiDive.Entities.Terrain;
using NijiDive.Controls.Movement;
using NijiDive.Controls.UI;

namespace NijiDive.UI.Menu
{
    public class ShopUI : UIItemStore
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
            if (this.shop) this.shop.OnPlayerContact.RemoveListener(SetMenuControls);
            this.shop = shop;
            this.shop.OnPlayerContact.AddListener(SetMenuControls);
        }

        protected override void SetMenuControls(bool enabled)
        {
            if (UIManager.singleton.Player == null) return;

            UIManager.singleton.Player.GetControlType<Jumping>().SetEnabled(!enabled);

            var uiControl = UIManager.singleton.Player.GetControlType<UIControl>();

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

            var purchasedProduct = shop.TryPurchase(SelectedIndex);
            if (purchasedProduct)
            {
                GetOption<SpriteRenderer>(SelectedIndex).sprite = null;
                OnPurchase?.Invoke(purchasedProduct);
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
            var optionTransforms = GetOptionsTransforms();
            if (optionTransforms.Length != Shop.FOR_SALE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(optionTransforms)} must be the same as {nameof(Shop.FOR_SALE_COUNT)}");
            }

            base.OnValidate();
        }
    }
}