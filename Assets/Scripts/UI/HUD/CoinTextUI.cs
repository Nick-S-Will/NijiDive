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

        private void Awake()
        {
            LevelManager.singleton.OnLoadLevelPostStart.AddListener(() => CoinManager.singleton.OnCoinChange.AddListener(UpdateCoinText));
        }

        private void Start()
        {
            PauseManager.OnSetPause.AddListener(UpdateTextVisible);
        }

        private void UpdateCoinText()
        {
            coinText.SetText(CoinManager.CoinCount.ToString());
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