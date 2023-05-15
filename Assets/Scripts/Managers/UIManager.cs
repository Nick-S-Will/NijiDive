using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.UI;
using NijiDive.Controls.UI;
using NijiDive.Utilities;

namespace NijiDive.Managers.PlayerBased.UI
{
    public class UIManager : PlayerBasedManager
    {
        [SerializeField] private UIElement[] consistentGameUI;
        [SerializeField] private MeshRenderer[] backgroundTextMeshes;

        public static UIManager singleton;
        public static UnityEvent OnUIControlGiven = new UnityEvent();

        private void OnEnable()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(UIManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }
        }

        private void Awake()
        {
            GivePlayerUIControl();
            HideAllUI();

            LevelManager.OnLoadLevel.AddListener(ShowGameUIAfterWorld0);
            OnNewPlayer.AddListener(GivePlayerUIControl);
        }

        public override void Retry()
        {
            Player.Retry();
        }

        private void GivePlayerUIControl()
        {
            var uiControl = new UIControl(Player, true);

            Player.AddControlType(uiControl);

            OnUIControlGiven.Invoke();
        }
        private void RemovePlayerUIControl()
        {
            if (Player == null) return;

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
        [ContextMenu("Show Consistent UI")]
        public void ShowConsistentUI()
        {
            HideAllUI();
            foreach (var ui in consistentGameUI) ui.SetVisible(true);
        }

        [ContextMenu("Update Text Meshes' Sorting Layers")]
        public void UpdateTextMeshesSortingLayers()
        {
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers) meshRenderer.sortingLayerName = backgroundTextMeshes.Contains(meshRenderer) ? Constants.COMBO_TEXT_SORTING_LAYER : Constants.TEXT_MESH_SORTING_LAYER;

            ScriptingUtilities.SetDirty(meshRenderers);
        }

        private void SetGameUIVisible(bool visible)
        {
            foreach (var ui in consistentGameUI) ui.SetVisible(visible);
        }
        private void ShowGameUI() => SetGameUIVisible(true);
        private void HideGameUI() => SetGameUIVisible(false);

        private void ShowGameUIAfterWorld0()
        {
            if (LevelManager.WorldIndex == 0) return;

            LevelManager.OnLoadLevel.RemoveListener(ShowGameUIAfterWorld0);
            ShowGameUI();
        }

        private void OnDisable()
        {
            if (singleton != this) return;

            singleton = null;
        }
    }
}