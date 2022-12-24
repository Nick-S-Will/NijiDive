using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.Managers.Coins;
using NijiDive.Managers.Pausing;

namespace NijiDive.UI.HUD
{
    public class CoinTextUI : MonoBehaviour
    {
        [SerializeField] private TextUI coinText;
        [SerializeField] [Min(0f)] private float visibleDuration = 1f;

        private void Start()
        {
            LevelManager.singleton.OnLoadLevelPostStart.AddListener(AddUpdateCoinTextToCoinManagerOnCoinChange);
            PauseManager.OnSetPause.AddListener(UpdateTextVisible);
        }

        private void UpdateCoinText()
        {
            coinText.SetText(CoinManager.CoinCount.ToString());
            _ = coinText.SetVisible(visibleDuration);
        }

        private void AddUpdateCoinTextToCoinManagerOnCoinChange()
        {
            if (CoinManager.singleton == null) return;

            CoinManager.singleton.OnCoinChange.AddListener(UpdateCoinText);
        }

        private void UpdateTextVisible()
        {
            coinText.visibilityIsLocked = false;
            coinText.SetVisible(PauseManager.IsPaused);
            coinText.visibilityIsLocked = PauseManager.IsPaused;
        }
    }
}