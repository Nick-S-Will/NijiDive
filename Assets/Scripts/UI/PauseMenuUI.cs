using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Managers.Levels;
using NijiDive.Managers.UI;
using NijiDive.Controls.UI;

namespace NijiDive.UI
{
    public class PauseMenuUI : UIMenu
    {
        [SerializeField] private Vector2 optionBorderSize;
        [Space]
        [SerializeField] protected TextMesh titleText;
        [SerializeField] protected TextMesh levelNameText;
        [SerializeField] private UnityEvent[] optionEvents;
        [Space]
        [SerializeField] private UIMenu[] mutuallyExclusiveMenus;

        protected void Awake()
        {
            LevelManager.singleton.OnLoadLevel.AddListener(UpdateLevelNameText);
            LevelManager.singleton.OnLoadLevel.AddListener(EnableToggle);
            LevelManager.singleton.OnLoadUpgrading.AddListener(DisableToggle);

            foreach(var uiMenu in mutuallyExclusiveMenus)
            {
                uiMenu.OnOpen.AddListener(DisableToggle);
                uiMenu.OnClose.AddListener(EnableToggle);
            }
        }

        protected override void Start() => base.Start();
        
        private void UpdateLevelNameText()
        {
            levelNameText.text = LevelManager.singleton.GetLevelName();
        }

        protected override (Vector2, Vector2) GetSelectedPositionAndSize(Transform selectedTransform)
        {
            var textSize = (Vector2)selectedTransform.GetComponent<MeshRenderer>().bounds.size;
            var position = (Vector2)selectedTransform.position + (textSize.x / 2f * Vector2.right);
            var size = textSize + optionBorderSize;
            return (position, size);
        }

        protected override void SetMenuControls(bool enabled)
        {
            var player = UIManager.singleton.Player;
            if (player == null) return;

            player.SetBaseFeatures(!enabled);
            var uiControl = player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnSelect, uiControl.OnUp, uiControl.OnDown },
                new UnityAction[] { Select, NavigateUp, NavigateDown },
                enabled);
        }

        private void SetToggleControl(bool enabled)
        {
            if (UIManager.singleton.Player == null) return;

            var uiControl = UIManager.singleton.Player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnCancel },
                new UnityAction[] { ToggleMenu },
                enabled);
        }
        private void EnableToggle() => SetToggleControl(true);
        private void DisableToggle() => SetToggleControl(false);

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            titleText.gameObject.SetActive(visible);
            levelNameText.gameObject.SetActive(visible);
        }

        public void ToggleMenu()
        {
            SetVisible(!IsVisible);

            SetMenuControls(IsVisible);

            PauseManager.PauseAll(IsVisible);
        }

        #region UI Control
        public override void Select()
        {
            optionEvents[SelectedIndex]?.Invoke();
        }

        private void Navigate(int indexDirection)
        {
            var optionCount = GetOptionCount();
            SelectedIndex = (SelectedIndex + indexDirection + optionCount) % optionCount;
            UpdateSelectedGraphics();

            OnNavigate?.Invoke();
        }
        public override void NavigateUp() => Navigate(-1);
        public override void NavigateDown() => Navigate(1);
        #endregion

        #region Pause Options
        public void Retry()
        {
            ToggleMenu();

            LevelManager.singleton.Restart();
        }
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

            var optionCount = GetOptionCount();
            if (optionEvents.Length != optionCount)
            {
                Array.Resize(ref optionEvents, optionCount);
            }
        }
    }
}