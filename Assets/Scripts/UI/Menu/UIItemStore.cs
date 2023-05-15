using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.MenuItems;

namespace NijiDive.UI.Menu
{
    public abstract class UIItemStore : UIMenu
    {
        public UnityEvent<MenuItem> OnPurchase;
        public UnityEvent OnBroke;
        [SerializeField] protected TextMesh itemNameText, itemDescriptionText;

        protected override void Start() => base.Start();

        protected void UpdateMenuItemSprites(MenuItem[] items)
        {
            var itemRenderers = GetOptions<SpriteRenderer>();
            for (int i = 0; i < itemRenderers.Length; i++)
            {
                if (items[i] == null) continue;

                try { itemRenderers[i].sprite = items[i].MenuUISprite; }
                catch (NullReferenceException) { print($"Renderer: {itemRenderers[i]}, Item: {items[i]}"); }
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