using UnityEngine;

using NijiDive.Managers.PlayerBased;
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

            AddListenersToPlayer();
            UpdateAmmoBar();

            PlayerBasedManager.OnNewPlayer.AddListener(AddListenersToPlayer);
        }

        private void AddListenersToPlayer()
        {
            playerWeaponController = PlayerBasedManager.Player.GetControlType<WeaponController>();
            playerWeaponController.OnEquip.AddListener(weapon => UpdateAmmoBar());
            playerWeaponController.OnShoot.AddListener(UpdateAmmoBar);
            playerWeaponController.OnReload.AddListener(UpdateAmmoBar);
        }

        private void UpdateAmmoBar()
        {
            ammoBar.SetBarFill(playerWeaponController.GetLeftInClip(), playerWeaponController.GetClipSize());
        }
    }
}