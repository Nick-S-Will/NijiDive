using UnityEngine;

using NijiDive.Controls.Player;
using NijiDive.Health;

namespace NijiDive.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private BarUI healthBar, bonusHealthBar;

        private PlayerHealthData playerHealth;

        private void Start()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found in scene", this);
                return;
            }
            if (healthBar == null || bonusHealthBar == null)
            {
                Debug.LogError($"{nameof(healthBar)} and {nameof(bonusHealthBar)} must be assigned", this);
                return;
            }

            playerHealth = (PlayerHealthData)player.Health;
            playerHealth.OnChangeHealth.AddListener(UpdateHealthBar);
        }

        private void UpdateHealthBar()
        {
            healthBar.SetBarFill((float)playerHealth.Health / playerHealth.MaxHealth);
            bonusHealthBar.SetBarFill((float)playerHealth.BonusHealth / playerHealth.MaxBonusHealth);
        }
    }
}