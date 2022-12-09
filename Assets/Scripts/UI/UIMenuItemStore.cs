using UnityEngine;
using UnityEngine.Events;

using NijiDive.MenuItems;

namespace NijiDive.UI
{
    public abstract class UIMenuItemStore : UIMenu
    {
        public UnityEvent<int> OnPurchase;
        public UnityEvent OnBroke;
        [SerializeField] protected TextMesh itemNameText, itemDescriptionText;

        protected override void Start() => base.Start();

        protected void UpdateMenuItemSprites(MenuItem[] items)
        {
            var itemRenderers = GetOptions<SpriteRenderer>();
            for (int i = 0; i < itemRenderers.Length; i++)
            {
                itemRenderers[i].sprite = items[i].StoreUISprite;
            }

            UpdateSelectedGraphicsAndText(items[SelectedIndex]);
        }

        protected void UpdateSelectedGraphicsAndText(MenuItem selectedItem)
        {
            UpdateSelectedGraphics();
            itemNameText.text = selectedItem ? selectedItem.name : string.Empty;
            itemDescriptionText.text = selectedItem ? selectedItem.Description : string.Empty;
        }

        public override void SetVisible(bool visible)
        {
            itemNameText.gameObject.SetActive(visible);
            itemDescriptionText.gameObject.SetActive(visible);

            base.SetVisible(visible);
        }
    }
}