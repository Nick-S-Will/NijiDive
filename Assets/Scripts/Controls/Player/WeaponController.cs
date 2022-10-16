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
        [Space]
        public UnityEvent<Weapon> OnEquip;
        public UnityEvent OnShoot, OnEmpty;

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

        public void EquipWeapon(Weapon newWeapon)
        {
            if (currentWeapon != null)
            {
                movement.OnLand.RemoveListener(currentWeapon.Reload);
                OnShoot.RemoveListener(currentWeapon.CompleteVolley);
            }

            currentWeapon = newWeapon;
            movement.OnLand.AddListener(currentWeapon.Reload);
            OnShoot.AddListener(currentWeapon.CompleteVolley);

            OnEquip?.Invoke(currentWeapon);
        }

        private void TryShoot()
        {
            if (shooting == null && currentWeapon.CanShoot()) shooting = StartCoroutine(ShootingRoutine());
        }
        private IEnumerator ShootingRoutine()
        {
            do
            {
                for (int burst = 0; burst < Mathf.Min(currentWeapon.LeftInClip, currentWeapon.ProjectilesPerBurst); burst++)
                {
                    for (int volley = 0; volley < currentWeapon.ProjectilesPerVolley; volley++) ShootSingle();
                    OnShoot?.Invoke();

                    if (currentWeapon.ClipIsEmpty)
                    {
                        OnEmpty?.Invoke();
                        goto endShooting;
                    }
                    else if (currentWeapon.ProjectilesPerBurst > 1) yield return new WaitForSeconds(currentWeapon.BurstProjectileInterval);
                }

                if (currentWeapon.IsAutomatic)
                {
                    yield return new WaitForSeconds(currentWeapon.ShotInterval);
                    if (!Input.GetKey(movement.JumpKey) || movement.IsOnGround) break;
                }
            } while (currentWeapon.IsAutomatic && currentWeapon.LeftInClip > 0);

        endShooting:
            shooting = null;
        }
        private void ShootSingle()
        {
            var height = (currentWeapon.Projectile.transform.lossyScale.x + currentWeapon.Projectile.transform.lossyScale.y) / 2f;
            var position = (Vector2)transform.position + (height * Vector2.down);
            var rotation = Quaternion.Euler(0, 0, (1f - currentWeapon.Accuracy) * Random.Range(-90f, 90f));
            var projectile = Instantiate(currentWeapon.Projectile, position, rotation);
            projectile.SetSpeed(currentWeapon.ProjectileSpeed - movement.GetVelocity().y);
            movement.SetVelocityY(currentWeapon.RecoilSpeed);
        }
    }
}