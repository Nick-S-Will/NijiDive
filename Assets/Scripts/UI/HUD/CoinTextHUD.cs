using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.Managers.Coins;
using NijiDive.Managers.Pausing;

namespace NijiDive.UI.HUD
{
    public class CoinTextHUD : MonoBehaviour
    {
        [SerializeField] private TextHUD coinText;
        [SerializeField] [Min(0f)] private float visibleDuration = 1f;

        private void Start()
        {
            CoinManager.OnCoinChange.AddListener(UpdateCoinText);
            PauseManager.OnSetPause.AddListener(UpdateTextVisible);
        }

        private void UpdateCoinText()
        {
            coinText.SetText(CoinManager.CoinCount.ToString());
            _ = coinText.SetVisible(visibleDuration);
        }

        private void UpdateTextVisible()
        {
            coinText.visibilityIsLocked = false;
            coinText.SetVisible(PauseManager.IsPaused);
            coinText.visibilityIsLocked = PauseManager.IsPaused;
        }

        private void OnDestroy()
        {
            CoinManager.OnCoinChange.RemoveListener(UpdateCoinText);
            PauseManager.OnSetPause.RemoveListener(UpdateTextVisible);
        }
    }
}