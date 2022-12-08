using UnityEngine;
using UnityEngine.Events;

using NijiDive.MenuItems;

namespace NijiDive.UI
{
    public abstract class UIMenuItemStore : UIMenu
    {
        public UnityEvent<int> OnPurchase;
        public UnityEvent OnBroke;
        [SerializeField] protected SpriteRenderer[] itemSpriteRenderers;
        [SerializeField] protected TextMesh itemNameText, itemDescriptionText;
        [Space]
        [SerializeField] [Min(0f)] private float slotSpacing;

        protected override void Start() => base.Start();
        
        protected override Vector3 GetSelectedPosition() => itemSpriteRenderers[SelectedIndex].transform.position;

        protected void UpdateSlotPositions()
        {
            for (int i = 0; i < itemSpriteRenderers.Length; i++)
            {
                var xPosition = (i * slotSpacing) - ((itemSpriteRenderers.Length - 1) / 2f * slotSpacing);
                itemSpriteRenderers[i].transform.localPosition = xPosition * Vector2.right;
            }

            UpdateSelectedGraphics();
        }

        protected void UpdateMenuItemSprites(MenuItem[] items)
        {
            for (int i = 0; i < itemSpriteRenderers.Length; i++)
            {
                itemSpriteRenderers[i].sprite = items[i].StoreUISprite;
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
            foreach (var sr in itemSpriteRenderers) sr.gameObject.SetActive(visible);
            itemNameText.gameObject.SetActive(visible);
            itemDescriptionText.gameObject.SetActive(visible);

            base.SetVisible(visible);
        }

        protected virtual void OnValidate()
        {
            UpdateSlotPositions();
        }
    }
}