using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.UI;
using NijiDive.Controls.UI;
using NijiDive.Entities.Mobs.Player;

namespace NijiDive.Managers.UI
{
    public class UIManager : Manager
    {
        [SerializeField] private UIElement[] consistentGameUI;

        public PlayerController Player { get; private set; }

        public static UIManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(UIManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }

            Player = FindObjectOfType<PlayerController>();

            HideAllUI();
            if (LevelManager.singleton) LevelManager.singleton.OnLoadLevel.AddListener(ShowGameUIAfterWorld0);
        }

        private void Start()
        {
            GivePlayerUIControl();
        }

        public override void Retry()
        {
            Player.Retry();
        }

        private void GivePlayerUIControl()
        {
            var uiControl = new UIControl(Player, true);

            Player.AddControlType(uiControl);
        }
        private void RemovePlayerUIControl()
        {
            if (Player == null) return; // No error since player may have been destroyed

            _ = Player.RemoveControlType<UIControl>();
        }

        public void SetAllUIVisible(bool visible)
        {
            foreach (var ui in GetComponentsInChildren<UIElement>(true)) ui.SetVisible(visible);
        }
        [ContextMenu("Show All UI")]
        public void ShowAllUI() => SetAllUIVisible(true);
        [ContextMenu("Hide All UI")]
        public void HideAllUI() => SetAllUIVisible(false);

        private void SetGameUIVisible(bool visible)
        {
            foreach (var ui in consistentGameUI) ui.SetVisible(visible);
        }
        private void ShowGameUI() => SetGameUIVisible(true);
        private void HideGameUI() => SetGameUIVisible(false);

        private void ShowGameUIAfterWorld0()
        {
            if (LevelManager.singleton.WorldIndex == 0) return;

            LevelManager.singleton.OnLoadLevel.RemoveListener(ShowGameUIAfterWorld0);
            ShowGameUI();
        }

        private void OnDestroy()
        {
            if (singleton != this) return;

            RemovePlayerUIControl();
            singleton = null;
        }
    }
}