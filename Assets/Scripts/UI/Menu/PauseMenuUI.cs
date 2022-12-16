using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Managers.Levels;
using NijiDive.Managers.UI;
using NijiDive.Controls.UI;
using NijiDive.Managers.Persistence;
using NijiDive.Managers.Coins;
using NijiDive.Managers.Combo;

namespace NijiDive.UI.Menu
{
    public class PauseMenuUI : UIMenu
    {
        [Space]
        [SerializeField] protected TextMesh titleText;
        [SerializeField] protected TextMesh levelNameText;
        [SerializeField] private UIMenu[] mutuallyExclusiveMenus;
        [Space]
        [SerializeField] private UnityEvent[] optionEvents;

        private bool toggleEnabled;

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

        protected override void SetMenuControls(bool enabled)
        {
            var player = UIManager.singleton.Player;
            if (player == null) return;

            var uiControl = player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnSelect, uiControl.OnUp, uiControl.OnDown },
                new UnityAction[] { Select, NavigateUp, NavigateDown },
                enabled);
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            titleText.gameObject.SetActive(visible);
            levelNameText.gameObject.SetActive(visible);
        }

        #region Toggle
        public void ToggleMenu()
        {
            SetVisible(!IsVisible);

            SetMenuControls(IsVisible);

            PauseManager.PauseAll(IsVisible);
        }

        private void SetToggleControl(bool enabled)
        {
            if (UIManager.singleton.Player == null || toggleEnabled == enabled) return;

            var uiControl = UIManager.singleton.Player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnCancel },
                new UnityAction[] { ToggleMenu },
                enabled);

            toggleEnabled = enabled;
        }
        private void EnableToggle() => SetToggleControl(true);
        private void DisableToggle() => SetToggleControl(false);
        #endregion

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

        #region Menu Options
        public void Retry()
        {
            ToggleMenu();

            // TODO: Create parent class for managers with a reset event to do this
            UIManager.singleton.Player.Retry();
            LevelManager.singleton.Retry();
            CoinManager.Restart();
            ComboManager.singleton.Retry();
        }

        // TODO: Add remaining option methods
        public void RestartToSurface()
        {
            PersistenceManager.Restart();
            LevelManager.singleton.RestartToSurface();
        }
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

            var optionCount = GetOptionCount();
            if (optionEvents.Length != optionCount)
            {
                Debug.LogWarning($"{nameof(optionEvents)} must have the same number of elements as options objects.");
                Array.Resize(ref optionEvents, optionCount);
            }
        }
    }
}