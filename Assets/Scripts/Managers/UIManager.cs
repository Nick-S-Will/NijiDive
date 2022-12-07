using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.Controls.UI;
using NijiDive.Controls.Player;

namespace NijiDive.Managers.UI
{
    public class UIManager : MonoBehaviour
    {
        public PlayerController Player { get; private set; }

        public static UIManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(UIManager)}s found in scene", this);
                gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            SetAllVisibleUI(false);

            Player = FindObjectOfType<PlayerController>();
            GivePlayerUIControl();
        }

        private void GivePlayerUIControl()
        {
            var uiControl = new UIControl();
            uiControl.mob = Player;
            uiControl.enabled = false;

            Player.AddControlType(uiControl);
        }
        private void RemovePlayerUIControl()
        {
            if (Player == null) return; // No error since player may have been destroyed

            _ = Player.RemoveControlType<UIControl>();
        }

        public void SetAllVisibleUI(bool visible)
        {
            foreach (var ui in GetComponentsInChildren<NijiDive.UI.UI>(true)) ui.SetVisible(visible);
        }
        [ContextMenu("Show All UI")]
        public void ShowAllUI() => SetAllVisibleUI(true);
        [ContextMenu("Hide All UI")]
        public void HideAllUI() => SetAllVisibleUI(false);


        private void OnDestroy()
        {
            if (singleton != this) return;
                
            RemovePlayerUIControl();
            singleton = null;
        }
    }
}