using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.UI;

namespace NijiDive.UI
{
    public abstract class UIMenu : UIBase
    {
        public UnityEvent OnOpen, OnClose, OnNavigate;
        [Space]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private SpriteRenderer itemSelectorRenderer;

        public int SelectedIndex { get; protected set; }

        protected virtual void Start()
        {
            OnOpen.AddListener(UIManager.singleton.Player.Disable);
            OnClose.AddListener(UIManager.singleton.Player.Enable);
        }

        protected abstract Vector3 GetSelectedPosition();
        protected void UpdateSelectedGraphics()
        {
            itemSelectorRenderer.transform.position = GetSelectedPosition();
        }

        public override bool IsVisible => menuPanel.activeSelf;
        public override void SetVisible(bool visible)
        {
            menuPanel.SetActive(visible);
            itemSelectorRenderer.gameObject.SetActive(visible);

            if (!Application.isPlaying) return;

            if (visible) OnOpen?.Invoke();
            else OnClose.Invoke();
        }

        protected void SetEventListeners(UnityEvent[] events, UnityAction[] actions, bool add)
        {
            if (events.Length != actions.Length)
            {
                Debug.LogError($"{nameof(UIMenu)} \"{name}\" is trying to set {actions.Length} {nameof(actions)} to {events.Length} {nameof(events)}.", this);
                return;
            }

            if (add) for (int i = 0; i < events.Length; i++) events[i].AddListener(actions[i]);
            else for (int i = 0; i < events.Length; i++) events[i].RemoveListener(actions[i]);
        }
        protected abstract void SetMenuControls(bool enabled);

        public abstract void Select();
        public virtual void NavigateLeft() { }
        public virtual void NavigateRight() { }
        public virtual void NavigateDown() { }
        public virtual void NavigateUp() { }
    }
}