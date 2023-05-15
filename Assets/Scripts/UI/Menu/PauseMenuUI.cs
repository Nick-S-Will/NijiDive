using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers;
using NijiDive.Managers.Levels;
using NijiDive.Managers.PlayerBased;
using NijiDive.Managers.PlayerBased.UI;
using NijiDive.Managers.Pausing;
using NijiDive.Managers.Persistence;
using NijiDive.Controls.UI;

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

        protected void Awake()
        {
            LevelManager.OnLoadLevel.AddListener(UpdateLevelNameText);
            UIManager.OnUIControlGiven.AddListener(EnableToggle);

            foreach(var uiMenu in mutuallyExclusiveMenus)
            {
                uiMenu.OnOpen.AddListener(DisableToggle);
                uiMenu.OnClose.AddListener(EnableToggle);
            }
        }

        protected override void Start()
        {
            base.Start();

            EnableToggle();
        }
        
        private void UpdateLevelNameText()
        {
            levelNameText.text = LevelManager.singleton.GetCurrentLevelName();
        }

        protected override void SetMenuControls(bool enabled)
        {
            var uiControl = PlayerBasedManager.Player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnSelect, uiControl.OnUp, uiControl.OnDown },
                new UnityAction[] { Select, NavigateUp, NavigateDown },
                enabled);
        }

        public override void SetVisible(bool visible)
        {
            titleText.gameObject.SetActive(visible);
            levelNameText.gameObject.SetActive(visible);

            base.SetVisible(visible);
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
            var uiControl = PlayerBasedManager.Player.GetControlType<UIControl>();

            SetEventListeners(
                new UnityEvent[] { uiControl.OnCancel },
                new UnityAction[] { ToggleMenu },
                enabled);
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

            Manager.RetryAllManagers();
        }

        public void ToCharacterSelect()
        {
            RestartToSurface();

            LevelManager.singleton.CompleteLevel();
        }

        public void ShowOptionsMenu()
        {
            print("Not yet implemented");
            // TODO: Implement game options
        }

        public void RestartToSurface()
        {
            PersistenceManager.singleton.Restart();
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