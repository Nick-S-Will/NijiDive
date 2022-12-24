using UnityEngine;

using NijiDive.Entities.Mobs.Player;
using NijiDive.Controls.Attacks;
using NijiDive.Weaponry;

namespace NijiDive.UI.HUD
{
    public class AmmoBarUI : MonoBehaviour
    {
        [SerializeField] private BarUI ammoBar;

        private WeaponController playerWeaponController;

        private void Start()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found in scene", this);
                return;
            }
            if (ammoBar == null)
            {
                Debug.LogError($"{nameof(ammoBar)} must be assigned", this);
                return;
            }

            playerWeaponController = player.GetControlType<WeaponController>();
            playerWeaponController.OnEquip.AddListener(UpdateAmmoBar);
            playerWeaponController.OnShoot.AddListener(UpdateAmmoBar);
            playerWeaponController.OnReload.AddListener(UpdateAmmoBar);

            UpdateAmmoBar();
        }

        private void UpdateAmmoBar(Weapon _) => UpdateAmmoBar();
        private void UpdateAmmoBar()
        {
            ammoBar.SetBarFill(playerWeaponController.GetLeftInClip(), playerWeaponController.GetClipSize());
        }
    }
}