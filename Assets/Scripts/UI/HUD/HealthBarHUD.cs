using UnityEngine;

using NijiDive.Entities.Mobs.Player;
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

            LookForPlayerAndAddListeners();
            UpdateHealthBar();
        }

        private void LookForPlayerAndAddListeners()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found", this);
                return;
            }

            playerHealth = (PlayerHealthData)player.Health;
            playerHealth.OnChangeHealth.AddListener(UpdateHealthBar);
        }

        private void UpdateHealthBar()
        {
            if (playerHealth == null) LookForPlayerAndAddListeners();

            healthBar.SetBarFill(playerHealth.Health, playerHealth.MaxHealth);
            bonusHealthBar.SetBarFill(playerHealth.BonusHealth, playerHealth.MaxBonusHealth);
        }
    }
}