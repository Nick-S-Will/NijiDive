using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.UI;
using NijiDive.Controls.UI;
using NijiDive.Entities.Mobs.Player;

namespace NijiDive.Managers.UI
{
    public class UIManager : Manager
    {
        public UnityEvent OnNewPlayer;

        [SerializeField] private UIElement[] consistentGameUI;

        private PlayerController player;

        public PlayerController Player 
        { 
            get
            {
                if (player == null)
                {
                    player = FindObjectOfType<PlayerController>();
                    if (player == null) Debug.LogError($"No {nameof(PlayerController)} found", this);
                    else OnNewPlayer?.Invoke();
                }
                return player;
            }
        }

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
        }

        private void Start()
        {
            GivePlayerUIControl();
            HideAllUI();
            if (LevelManager.singleton) LevelManager.singleton.OnLoadLevel.AddListener(ShowGameUIAfterWorld0);
        }

        public override void Retry()
        {
            player.Retry();
        }

        private void GivePlayerUIControl()
        {
            var uiControl = new UIControl(player, true);

            player.AddControlType(uiControl);
        }
        private void RemovePlayerUIControl()
        {
            if (player == null) return;

            _ = player.RemoveControlType<UIControl>();
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