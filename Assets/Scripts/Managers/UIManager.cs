using UnityEngine;
using UnityEngine.Events;

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
            if (LevelManager.singleton.WorldIndex == 0) gameObject.SetActive(false);

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

        private void OnDestroy()
        {
            if (singleton != this) return;
                
            singleton = null;
            RemovePlayerUIControl();
        }
    }
}