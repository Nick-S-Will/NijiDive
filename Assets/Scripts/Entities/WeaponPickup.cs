using System;
using UnityEngine;

using NijiDive.Controls.Attacks;
using NijiDive.Weaponry;
using NijiDive.MenuItems;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class WeaponPickup : Entity
    {
        [SerializeField] private float flashInterval = 1f;
        [Space]
        [SerializeField] private SpriteRenderer pickupBackgroundRenderer;
        [SerializeField] private TextMesh weaponNameTextMesh;
        [Space]
        [SerializeField] private Sprite healthBackground;
        [SerializeField] private int healthBuff = 1;
        [Space]
        [SerializeField] private Sprite ammoBackground;
        [SerializeField] private int ammoBuff = 2;
        [Space]
        [SerializeField] private Weapon[] weaponOptions;

        private Weapon selectedWeapon;
        private BuffType buffType;

        private void Start()
        {
            if (weaponOptions.Length == 0)
            {
                Debug.LogError($"{nameof(weaponOptions)} is empty", this);
                return;
            }

            selectedWeapon = weaponOptions[UnityEngine.Random.Range(0, weaponOptions.Length)];
            weaponNameTextMesh.text = selectedWeapon.name.Substring(0, 1);

            var buffTypes = new BuffType[] { BuffType.Health, BuffType.Ammo };
            buffType = buffTypes[UnityEngine.Random.Range(0, buffTypes.Length)];
            pickupBackgroundRenderer.sprite = buffType == BuffType.Health ? healthBackground : ammoBackground;

            PeriodicFlashing(pickupBackgroundRenderer, flashInterval);
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

                switch (buffType)
                {
                    case BuffType.Health:
                        mob.Health.ReceiveHealth(healthBuff);
                        break;
                    case BuffType.Ammo:
                        if (weaponController != null) weaponController.AddBonusAmmo(ammoBuff);
                        break;
                }
            }
        }
    }
}