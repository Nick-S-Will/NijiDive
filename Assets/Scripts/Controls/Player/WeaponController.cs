using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

using NijiDive.Weapons;

namespace NijiDive.Controls.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Weapon startingWeapon;

        public UnityEvent OnShoot;

        private PlayerController movement;
        private Weapon currentWeapon;
        private Coroutine shooting;

        private void Start()
        {
            movement = GetComponent<PlayerController>();

            if (startingWeapon == null)
            {
                Debug.LogError("No starting weapon assigned");
                enabled = false;
            }
            else EquipWeapon(startingWeapon);
        }

        private void Update()
        {
            if (Input.GetKeyDown(movement.JumpKey) && !movement.IsOnGroundRaw) TryShoot();
        }

        private void EquipWeapon(Weapon newWeapon)
        {
            if (currentWeapon != null)
            {
                movement.OnLand.RemoveListener(currentWeapon.Reload);
                OnShoot.RemoveListener(currentWeapon.CompleteVolley);
            }

            currentWeapon = newWeapon;
            movement.OnLand.AddListener(currentWeapon.Reload);
            OnShoot.AddListener(currentWeapon.CompleteVolley);
        }

        private void TryShoot()
        {
            if (shooting == null && currentWeapon.CanShoot()) shooting = StartCoroutine(ShootingRoutine());
        }
        private IEnumerator ShootingRoutine()
        {
            do
            {
                for (int burst = 0; burst < currentWeapon.ProjectilesPerBurst; burst++)
                {
                    for (int volley = 0; volley < Mathf.Min(currentWeapon.LeftInClip, currentWeapon.ProjectilesPerVolley); volley++)
                    {
                        var rotation = Quaternion.Euler(0, 0, (1f - currentWeapon.Accuracy) * Random.Range(-90f, 90f));
                        var projectile = Instantiate(currentWeapon.Projectile, transform.position, rotation);
                        projectile.SetSpeed(currentWeapon.ProjectileSpeed);
                        movement.Rb2d.velocity += currentWeapon.RecoilSpeed * Vector2.up;
                    }
                    OnShoot?.Invoke();

                    if (currentWeapon.ClipIsEmpty) goto endShooting;
                    else if (currentWeapon.ProjectilesPerBurst > 1) yield return new WaitForSeconds(currentWeapon.BurstProjectileInterval);
                }

                if (currentWeapon.IsAutomatic)
                {
                    yield return new WaitForSeconds(currentWeapon.ShotInterval);
                    if (!Input.GetKey(movement.JumpKey)) break;
                }
            } while (currentWeapon.IsAutomatic && currentWeapon.LeftInClip > 0);

            endShooting:
            shooting = null;
        }
    }
}