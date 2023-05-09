using UnityEngine;

using NijiDive.Entities.Mobs.Player;
using NijiDive.Controls.Attacks;

namespace NijiDive.UI.HUD
{
    public class AmmoBarHUD : MonoBehaviour
    {
        [SerializeField] private BarHUD ammoBar;

        private WeaponController playerWeaponController;

        private void Start()
        {
            if (ammoBar == null)
            {
                Debug.LogError($"{nameof(ammoBar)} must be assigned", this);
                return;
            }

            LookForPlayerAndAddListeners();
            UpdateAmmoBar();
        }

        private void LookForPlayerAndAddListeners()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found", this);
                return;
            }

            playerWeaponController = player.GetControlType<WeaponController>();
            playerWeaponController.OnEquip.AddListener(weapon => UpdateAmmoBar());
            playerWeaponController.OnShoot.AddListener(UpdateAmmoBar);
            playerWeaponController.OnReload.AddListener(UpdateAmmoBar);
        }

        private void UpdateAmmoBar()
        {
            if (playerWeaponController == null) LookForPlayerAndAddListeners();

            ammoBar.SetBarFill(playerWeaponController.GetLeftInClip(), playerWeaponController.GetClipSize());
        }
    }
}