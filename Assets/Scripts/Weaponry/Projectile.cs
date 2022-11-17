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

        private WeaponController source;
        private Rigidbody2D rb2d;
        private float startTime;

        private void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            startTime = Time.time;

            Destroy(gameObject, lifeTime);
        }

        public void Setup(WeaponController source, float magnitude)
        {
            this.source = source;

            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
            rb2d.velocity = magnitude * -transform.up;
        }

        /*private void FixedUpdate()
        {
            var hitInfo = Physics2D.Raycast(transform.position, -transform.up, rb2d.velocity.y * Time.fixedDeltaTime);
            if (hitInfo.collider != null) TryDamage(hitInfo.collider, hitInfo.point);
        }*/

        // Switched to trigger instead of raycast because raycast doesn't work with composite collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Time.time - startTime > Time.fixedDeltaTime && Attacking.TryDamageCollider(gameObject, collision, damageType, damage, collision.ClosestPoint(transform.position)))
            {
                source.OnDamage?.Invoke();
                if (Attacking.IsDead(collision)) source.OnKill?.Invoke();
            }

            Destroy(gameObject);
        }
    }
}