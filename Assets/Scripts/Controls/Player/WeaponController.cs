using System.Collections;
using UnityEngine.Events;
using UnityEngine;

using NijiDive.Weapons;

namespace NijiDive.Controls.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Weapon startingWeapon;
        [SerializeField] private LayerMask damageLayers;
        [Space]
        public UnityEvent<Weapon> OnEquip;
        public UnityEvent OnShoot, OnEmpty, OnStomp;

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

        private void FixedUpdate()
        {
            var bounds = movement.GroundCheckBounds;
            var collider = Physics2D.OverlapBox(bounds.center, bounds.size, 0f, damageLayers);
            if (collider) TryDamage(collider, collider.ClosestPoint(transform.position));
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
            var position = transform.position;
            var rotation = Quaternion.Euler(0, 0, (1f - currentWeapon.Accuracy) * Random.Range(-90f, 90f));
            var projectile = Instantiate(currentWeapon.Projectile, position, rotation);
            projectile.SetSpeed(currentWeapon.ProjectileSpeed - movement.GetVelocity().y);
            movement.SetVelocityY(currentWeapon.RecoilSpeed);
        }

        private void TryDamage(Collider2D collider, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                _ = damageable.TakeDamage(int.MaxValue, DamageType.Player | DamageType.Contact, point);
                OnStomp?.Invoke();
            }
        }
    }
}