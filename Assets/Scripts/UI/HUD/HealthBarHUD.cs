using UnityEngine;

using NijiDive.Managers.PlayerBased;
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

            PlayerBasedManager.OnNewPlayer.AddListener(AddListenersToPlayer);
        }

        private void AddListenersToPlayer()
        {
            playerHealth = (PlayerHealthData)PlayerBasedManager.Player.Health;
            playerHealth.OnChangeHealth.AddListener(UpdateHealthBar);
        }

        private void UpdateHealthBar()
        {
            healthBar.SetBarFill(playerHealth.HealthPoints, playerHealth.MaxHealthPoints);
            bonusHealthBar.SetBarFill(playerHealth.BonusHealth, playerHealth.MaxBonusHealth);
        }
    }
}