using UnityEngine;

using NijiDive.Controls.Attacks;
using NijiDive.Weaponry;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class WeaponPickup : Entity
    {
        [SerializeField] private TextMesh weaponNameText;
        [Space]
        [SerializeField] private Weapon[] weaponOptions;

        private Weapon selectedWeapon;

        private void Start()
        {
            if (weaponOptions.Length == 0)
            {
                Debug.LogError($"{nameof(weaponOptions)} is empty");
                return;
            }

            selectedWeapon = weaponOptions[Random.Range(0, weaponOptions.Length)];
            weaponNameText.text = selectedWeapon.name.Substring(0, 1);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var mob = collision.GetComponent<Mob>();
            if (mob)
            {
                var weaponController = mob.GetControlType<WeaponController>();
                if (weaponController != null)
                {
                    weaponController.EquipWeapon(selectedWeapon);
                    Destroy(gameObject);
                }
            }
        }
    }
}