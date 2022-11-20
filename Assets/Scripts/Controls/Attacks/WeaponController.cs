using System.Collections;
using UnityEngine.Events;
using UnityEngine;

using NijiDive.Weaponry;
using NijiDive.Controls.Movement;

namespace NijiDive.Controls.Attacks
{
    [System.Serializable]
    public class WeaponController : Attacking
    {
        [SerializeField] private Weapon startingWeapon;
        [Space]
        public UnityEvent<Weapon> OnEquip;
        public UnityEvent OnShoot, OnEmpty, OnReload;

        private Weapon currentWeapon;
        private Coroutine shooting;
        private int bonusAmmo;

        public override void Start()
        {
            var jumping = mob.GetControlType<Jumping>();
            if (jumping == default)
            {
                Debug.LogError($"{mob.name} is missing {typeof(Jumping)}. {typeof(WeaponController)} requires it", mob);
                mob.enabled = false;
            }
            else jumping.OnLand.AddListener(ReloadCurrentWeapon);
            var stomping = mob.GetControlType<Stomping>();
            if (stomping == default)
            {
                Debug.LogError($"{mob.name} is missing {typeof(Stomping)}. {typeof(WeaponController)} requires it", mob);
                mob.enabled = false;
            }
            else stomping.OnDamage.AddListener(ReloadCurrentWeapon);
            if (!mob.enabled) return;

            if (startingWeapon == null)
            {
                Debug.LogError("No starting weapon assigned", mob);
                mob.enabled = false;
            }
            else EquipWeapon(startingWeapon);
        }

        public override void FixedUpdate()
        {
            if (!mob.LastGroundCheck && mob.LastInputs.actionDownThisFrame) TryShoot();
        }

        public void EquipWeapon(Weapon newWeapon)
        {
            if (currentWeapon != null)
            {
                OnShoot.RemoveListener(currentWeapon.CompleteVolley);
            }

            currentWeapon = newWeapon;
            currentWeapon.Load();
            OnShoot.AddListener(currentWeapon.CompleteVolley);

            OnEquip?.Invoke(currentWeapon);
        }

        private void ReloadCurrentWeapon()
        {
            currentWeapon.Reload();
            OnReload?.Invoke();
        }
        public void AddBonusAmmo(int amount = 1)
        {
            bonusAmmo += amount;
            ReloadCurrentWeapon();
        }

        public int GetLeftInClip() => currentWeapon.LeftInClip + bonusAmmo;
        public int GetClipSize() => currentWeapon.ClipSize + bonusAmmo;

        #region Shooting
        private void TryShoot()
        {
            if (shooting == null && currentWeapon.CanShoot() && GetLeftInClip() > 0) shooting = mob.StartCoroutine(ShootingRoutine());
        }

        private IEnumerator ShootingRoutine()
        {
            do
            {
                for (int burst = 0; burst < Mathf.Min(GetLeftInClip(), currentWeapon.ProjectilesPerBurst); burst++)
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
                    if (!mob.LastInputs.actionDown || mob.LastGroundCheck) break;
                }
            } while (currentWeapon.IsAutomatic && GetLeftInClip() > 0);

        endShooting:
            shooting = null;
        }

        private void ShootSingle()
        {
            var position = mob.transform.position;
            var rotation = Quaternion.Euler(0, 0, (1f - currentWeapon.Accuracy) * Random.Range(-90f, 90f));
            var projectile = Object.Instantiate(currentWeapon.Projectile, position, rotation);
            projectile.Setup(this, currentWeapon.ProjectileSpeed);
            mob.SetVelocityY(currentWeapon.RecoilSpeed);
        }
        #endregion
    }
}