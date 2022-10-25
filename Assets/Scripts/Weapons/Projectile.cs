using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Weapons
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private int damage;
        [SerializeField] private DamageType damageType = DamageType.Player | DamageType.Projectile;
        [Space]

        public UnityEvent OnHit;

        private Rigidbody2D rb2d;
        private float startTime;

        private void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            startTime = Time.time;

            Destroy(gameObject, lifeTime);
        }

        public void SetSpeed(float magnitude)
        {
            if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
            rb2d.velocity = magnitude * -transform.up;
        }

        /*private void FixedUpdate()
        {
            var hitInfo = Physics2D.Raycast(transform.position, -transform.up, rb2d.velocity.y * Time.fixedDeltaTime);
            if (hitInfo.collider != null) TryDamage(hitInfo.collider, hitInfo.point);
        }*/

        private void TryDamage(Collider2D collider, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                _ = damageable.TryDamage(damage, damageType, point);
                OnHit?.Invoke();
                Destroy(gameObject);
            }
        }

        // Using trigger instead of raycast because raycast doesn't work with composite collider
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Time.time - startTime > Time.fixedDeltaTime) TryDamage(collision, collision.ClosestPoint(transform.position));
        }
    }
}