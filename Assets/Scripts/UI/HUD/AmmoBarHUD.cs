using UnityEngine;

using NijiDive.Managers.PlayerBased;
using NijiDive.Controls.Attacks;
using NijiDive.Controls.Attacks.Specials;

namespace NijiDive.UI.HUD
{
    public class AmmoBarHUD : MonoBehaviour
    {
        [SerializeField] private BarHUD ammoBar, chargeBar;

        private WeaponController playerWeaponController;
        private Special playerSpecial;

        private void Start()
        {
            if (ammoBar == null)
            {
                Debug.LogError($"{nameof(ammoBar)} must be assigned", this);
                return;
            }
            if (chargeBar == null)
            {
                Debug.LogError($"{nameof(chargeBar)} must be assigned", this);
                return;
            }

            PlayerBasedManager.OnNewPlayer.AddListener(AddListenersToPlayer);
        }

        private void AddListenersToPlayer()
        {
            playerWeaponController = PlayerBasedManager.Player.GetControlType<WeaponController>();
            playerWeaponController.OnEquip.AddListener(weapon => UpdateAmmoBar());
            playerWeaponController.OnShoot.AddListener(UpdateAmmoBar);
            playerWeaponController.OnReload.AddListener(UpdateAmmoBar);

            playerSpecial = PlayerBasedManager.Player.GetControlType<Special>();
            chargeBar.gameObject.SetActive(playerSpecial != null);
            if (playerSpecial == null) return;
            playerSpecial.OnCharge.AddListener(UpdateChargeBar);
            playerSpecial.OnUse.AddListener(UpdateChargeBar);
            UpdateChargeBar();
        }

        private void UpdateAmmoBar()
        {
            ammoBar.SetBarFill(playerWeaponController.GetLeftInClip(), playerWeaponController.GetClipSize());
        }

        private void UpdateChargeBar()
        {
            chargeBar.SetBarFill(playerSpecial.Charges, playerSpecial.MaxCharges);
        }
    }
}