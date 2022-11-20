using UnityEngine;

using NijiDive.Controls.Player;
using NijiDive.Controls.Attacks;
using NijiDive.Weaponry;

namespace NijiDive.UI
{
    public class AmmoBarUI : MonoBehaviour
    {
        [SerializeField] private BarUI ammo;

        private WeaponController playerWeaponController;

        private void Start()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError($"No {nameof(PlayerController)} found in scene", this);
                return;
            }
            if (ammo == null)
            {
                Debug.LogError($"{nameof(ammo)} must be assigned", this);
                return;
            }

            playerWeaponController = player.GetControlType<WeaponController>();
            playerWeaponController.OnEquip.AddListener(UpdateAmmoBar);
            playerWeaponController.OnShoot.AddListener(UpdateAmmoBar);
            playerWeaponController.OnReload.AddListener(UpdateAmmoBar);
        }

        private void UpdateAmmoBar(Weapon _) => UpdateAmmoBar();
        private void UpdateAmmoBar()
        {
            ammo.SetBarFill((float)playerWeaponController.GetLeftInClip() / playerWeaponController.GetClipSize());
        }
    }
}