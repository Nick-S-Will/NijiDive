using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.UI;

namespace NijiDive.UI.Menu
{
    public abstract class UIMenu : UIBase
    {
        public UnityEvent OnOpen, OnClose, OnNavigate;
        [Space]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private SpriteRenderer optionSelectorRenderer;
        [SerializeField] private Transform optionsParent;
        [Space]
        [SerializeField] private Vector2 optionSpacing = Vector2.right + Vector2.down;
        [SerializeField] private Vector2 optionBorderSize = 0.5f * Vector2.one;

        protected float OptionSpacingX => optionSpacing.x;
        protected float OptionSpacingY => optionSpacing.y;

        public int SelectedIndex { get; protected set; }

        protected virtual void Start()
        {
            OnOpen.AddListener(UIManager.singleton.Player.DisableBaseFeatures);
            OnOpen.AddListener(UpdateSelectedGraphics);
            OnClose.AddListener(UIManager.singleton.Player.EnableBaseFeatures);
        }

        protected int GetOptionCount() => optionsParent.childCount;

        protected T GetOption<T>(int index) where T : Component
        {
            if (optionsParent == null) return default;

            var child = optionsParent.GetChild(index);
            return child.GetComponent<T>();
        }

        protected T[] GetOptions<T>() where T : Component
        {
            if (optionsParent == null) return new T[0];

            return optionsParent.GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Gets all of <see cref="optionsParent"/>'s children. Used since <see cref="GetOptions{T}"/> would include the parent transform
        /// </summary>
        protected Transform[] GetOptionsTransforms()
        {
            if (optionsParent == null) return new Transform[0];

            return optionsParent.Cast<Transform>().ToArray();
        }

        protected virtual (Vector2, Vector2) GetSelectedPositionAndSize(Transform selectedTransform)
        {
            var textRenderer = selectedTransform.GetComponent<MeshRenderer>();

            if (textRenderer == null) return (selectedTransform.position, selectedTransform.lossyScale);

            var textSize = (Vector2)textRenderer.bounds.size;
            var position = (Vector2)selectedTransform.position + (textSize.x / 2f * Vector2.right);
            var size = textSize;
            return (position, size);
        }

        protected void UpdateSelectedGraphics()
        {
            if (optionsParent == null || SelectedIndex >= optionsParent.childCount) return;

            var positionSize = GetSelectedPositionAndSize(optionsParent.GetChild(SelectedIndex));
            optionSelectorRenderer.transform.position = positionSize.Item1;
            optionSelectorRenderer.size = positionSize.Item2 + optionBorderSize;
        }

        protected void UpdateOptionPositions()
        {
            var optionTransforms = GetOptionsTransforms();
            for (int i = 0; i < optionTransforms.Length; i++)
            {
                var xPosition = (i * OptionSpacingX) - ((optionTransforms.Length - 1) / 2f * OptionSpacingX);
                var yPosition = (i * OptionSpacingY) - ((optionTransforms.Length - 1) / 2f * OptionSpacingY);
                optionTransforms[i].localPosition = new Vector2(xPosition, yPosition);
            }

            UpdateSelectedGraphics();
        }

        [ContextMenu("Update Shape")]
        public override void UpdateShape()
        {
            base.UpdateShape();

            UpdateOptionPositions();
        }

        public override bool IsVisible => menuPanel.activeSelf;
        public override void SetVisible(bool visible)
        {
            menuPanel.SetActive(visible);
            optionSelectorRenderer.gameObject.SetActive(visible);
            optionsParent.gameObject.SetActive(visible);

            if (!Application.isPlaying) return;

            if (visible) OnOpen?.Invoke();
            else OnClose.Invoke();
        }

        /// <summary>
        /// Adds <paramref name="actions"/> as listeners to <paramref name="events"/> or removes them based on <paramref name="add"/>. Arrays must be the same length
        /// </summary>
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

        protected virtual void OnValidate()
        {
            UpdateOptionPositions();
        }
    }
}