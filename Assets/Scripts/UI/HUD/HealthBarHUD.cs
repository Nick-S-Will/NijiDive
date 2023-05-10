using UnityEngine;

using NijiDive.Managers.UI;
using NijiDive.Health;

namespace NijiDive.UI.HUD
{
    public class HealthBarHUD : MonoBehaviour
    {
        [SerializeField] private BarHUD healthBar, bonusHealthBar;

        private PlayerHealthData playerHealth;

        private void Start()
        {
            if (healthBar == null || bonusHealthBar == null)
            {
                Debug.LogError($"{nameof(healthBar)} and {nameof(bonusHealthBar)} must be assigned", this);
                return;
            }

            AddListenersToPlayer();
            UpdateHealthBar();

            UIManager.singleton.OnNewPlayer.AddListener(AddListenersToPlayer);
        }

        private void AddListenersToPlayer()
        {
            playerHealth = (PlayerHealthData)UIManager.singleton.Player.Health;
            playerHealth.OnChangeHealth.AddListener(UpdateHealthBar);
        }

        private void UpdateHealthBar()
        {
            healthBar.SetBarFill(playerHealth.Health, playerHealth.MaxHealth);
            bonusHealthBar.SetBarFill(playerHealth.BonusHealth, playerHealth.MaxBonusHealth);
        }
    }
}