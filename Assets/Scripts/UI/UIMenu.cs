using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.UI
{
    public abstract class UIMenu : UI
    {
        public UnityEvent OnOpen, OnClose, OnNavigate;
        [Space]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private SpriteRenderer itemSelectorRenderer;
        [SerializeField] protected SpriteRenderer[] itemSpriteRenderers;
        [SerializeField] protected TextMesh itemNameText, itemDescriptionText;
        [Space]
        [SerializeField] [Min(0f)] private float slotSpacing;

        public int SelectedIndex { get; protected set; }

        public override bool IsVisible => menuPanel.activeSelf;

        public void UpdateSlotPositions()
        {
            for (int i = 0; i < itemSpriteRenderers.Length; i++)
            {
                var xPosition = (i * slotSpacing) - ((itemSpriteRenderers.Length - 1) / 2f * slotSpacing);
                itemSpriteRenderers[i].transform.localPosition = xPosition * Vector2.right;
            }

            itemSelectorRenderer.transform.position = itemSpriteRenderers[0].transform.position;
        }

        protected void UpdateSelectedGraphics()
        {
            itemSelectorRenderer.transform.position = itemSpriteRenderers[SelectedIndex].transform.position;
        }

        public override void SetVisible(bool visible)
        {
            menuPanel.SetActive(visible);
            itemSelectorRenderer.gameObject.SetActive(visible);
            foreach (var sr in itemSpriteRenderers) sr.gameObject.SetActive(visible);
        }

        protected void SetEventListeners(UnityEvent[] events, UnityAction[] actions, bool add)
        {
            if (add) for (int i = 0; i < events.Length; i++) events[i].AddListener(actions[i]);
            else for (int i = 0; i < events.Length; i++) events[i].RemoveListener(actions[i]);
        }

        protected abstract void SetMenuControls(bool enabled);

        public abstract void Select();
        public virtual void NavigateLeft() { }
        public virtual void NavigateRight() { }
        public virtual void NavigateDown() { }
        public virtual void NavigateUp() { }

        protected virtual void OnValidate()
        {
            UpdateSlotPositions();
        }
    }
}