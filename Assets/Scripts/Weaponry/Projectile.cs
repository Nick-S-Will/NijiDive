using UnityEngine;

using NijiDive.Controls.Attacks;

namespace NijiDive.Weaponry
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float lifeTime = 0.2f;
        [SerializeField] [Min(1)] private int damage = 1;
        [SerializeField] private DamageType damageType = DamageType.Player | DamageType.Projectile;

        private WeaponController sourceWeaponController;
        private Rigidbody2D body2D;
        private float startTime;

        private void Start()
        {
            body2D = GetComponent<Rigidbody2D>();
            startTime = Time.time;

            Destroy(gameObject, lifeTime);
        }

        public void Setup(WeaponController source, float magnitude)
        {
            this.sourceWeaponController = source;

            if (body2D == null) body2D = GetComponent<Rigidbody2D>();
            body2D.velocity = magnitude * -transform.up;
        }

        // Switched to trigger from raycast because raycast doesn't work with composite collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Time.time - startTime > Time.fixedDeltaTime && Attacking.TryDamageCollider(this, collision, damageType, damage, collision.ClosestPoint(transform.position)))
            {
                sourceWeaponController.OnDamage?.Invoke();
                if (Attacking.IsDead(collision)) sourceWeaponController.OnKill?.Invoke();
            }

            Destroy(gameObject);
        }
    }
}