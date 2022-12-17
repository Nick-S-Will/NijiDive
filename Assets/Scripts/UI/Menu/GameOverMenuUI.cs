using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Persistence;
using NijiDive.Managers.Levels;
using NijiDive.Managers.Coins;
using NijiDive.Managers.Combo;
using NijiDive.Managers.UI;
using NijiDive.Controls.UI;

namespace NijiDive.UI.Menu
{
    public class GameOverMenuUI : UIMenu
    {
        [Space]
        [SerializeField] protected TextMesh titleText;
        [SerializeField] protected TextMesh levelNameText, coinCountText, killCountText, maxComboText;
        [Space]
        [SerializeField] private UnityEvent[] optionEvents;

        protected void Awake()
        {
            UIManager.singleton.Player.OnDeath.AddListener((monoBehaviour, damageType) => SetVisible(true));
            OnOpen.AddListener(UpdateStatTexts);
            OnOpen.AddListener(UpdateSelectedGraphics);
            OnOpen.AddListener(EnableMenuControls);
            OnClose.AddListener(DisableMenuControls);
        }

        private void UpdateStatTexts()
        {
            levelNameText.text = LevelManager.singleton.GetLevelName();
            coinCountText.text = CoinManager.TotalCoinCount.ToString();
            killCountText.text = ComboManager.singleton.TotalKillCount.ToString();
            maxComboText.text = ComboManager.singleton.MaxCombo.ToString();
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
        private void EnableMenuControls() => SetMenuControls(true);
        private void DisableMenuControls() => SetMenuControls(false);

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            titleText.gameObject.SetActive(visible);
            levelNameText.gameObject.SetActive(visible);
            coinCountText.gameObject.SetActive(visible);
            killCountText.gameObject.SetActive(visible);
            maxComboText.gameObject.SetActive(visible);
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

        #region Menu Options
        public void Retry()
        {
            SetVisible(false);
            SetMenuControls(false);

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