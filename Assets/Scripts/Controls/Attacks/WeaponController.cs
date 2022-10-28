using System.Collections;
using UnityEngine.Events;
using UnityEngine;

using NijiDive.Weapons;
using NijiDive.Controls.Movement;

namespace NijiDive.Controls.Attacks
{
    [System.Serializable] 
    public class WeaponController : Attacking
    {
        [SerializeField] private Weapon startingWeapon;
        [Space]
        public UnityEvent<Weapon> OnEquip;
        public UnityEvent OnShoot, OnEmpty;

        private Weapon currentWeapon;
        private Coroutine shooting;

        public override void Start()
        {
            if (startingWeapon == null)
            {
                Debug.LogError("No starting weapon assigned");
                mob.enabled = false;
            }
            else EquipWeapon(startingWeapon);

            if (mob.GetMovingType<Jumping>() == default)
            {
                Debug.LogError($"{mob.name} is missing {typeof(Jumping)} but {typeof(WeaponController)}");
                mob.enabled = false;
            }
        }

        public override void FixedUpdate()
        {
            if (!mob.lastGroundCheck && mob.LastInputs.actionDownThisFrame) TryShoot();
        }

        public void EquipWeapon(Weapon newWeapon)
        {
            var jumping = mob.GetMovingType<Jumping>();

            if (currentWeapon != null)
            {
                jumping.OnLand.RemoveListener(currentWeapon.Reload);
                OnShoot.RemoveListener(currentWeapon.CompleteVolley);
            }

            currentWeapon = newWeapon;
            jumping.OnLand.AddListener(currentWeapon.Reload);
            OnShoot.AddListener(currentWeapon.CompleteVolley);

            OnEquip?.Invoke(currentWeapon);
        }

        #region Shooting
        private void TryShoot()
        {
            if (shooting == null && currentWeapon.CanShoot()) shooting = mob.StartCoroutine(ShootingRoutine());
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
                    if (!mob.LastInputs.actionDown || mob.lastGroundCheck) break;
                }
            } while (currentWeapon.IsAutomatic && currentWeapon.LeftInClip > 0);

        endShooting:
            shooting = null;
        }

        private void ShootSingle()
        {
            var position = mob.transform.position;
            var rotation = Quaternion.Euler(0, 0, (1f - currentWeapon.Accuracy) * Random.Range(-90f, 90f));
            var projectile = Object.Instantiate(currentWeapon.Projectile, position, rotation);
            projectile.SetSpeed(currentWeapon.ProjectileSpeed);
            mob.SetVelocityY(currentWeapon.RecoilSpeed);
        }
        #endregion
    }
}