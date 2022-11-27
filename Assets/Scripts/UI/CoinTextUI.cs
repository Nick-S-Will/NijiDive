using UnityEngine;

using NijiDive.Managers.Coins;
using NijiDive.Managers.Pausing;

namespace NijiDive.UI
{
    public class CoinTextUI : MonoBehaviour
    {
        [SerializeField] private TextUI coinText;
        [SerializeField] [Min(0f)] private float visibleDuration = 1f;

        private void Start()
        {
            CoinManager.singleton.OnCoinChange.AddListener(UpdateCoinText);
            PauseManager.OnSetPause.AddListener(UpdateTextVisible);
        }

        private void UpdateCoinText()
        {
            coinText.SetText(CoinManager.singleton.CoinCount.ToString());
            coinText.SetVisible(visibleDuration);
        }

        private void UpdateTextVisible()
        {
            coinText.visibilityIsLocked = false;
            coinText.SetVisible(PauseManager.IsPaused);
            coinText.visibilityIsLocked = PauseManager.IsPaused;
        }
    }
}