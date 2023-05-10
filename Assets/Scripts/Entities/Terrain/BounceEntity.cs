using UnityEngine;

namespace NijiDive.Entities.Terrain
{
    public class BounceEntity : Entity, IDamageable
    {
        [Space]
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;
        [Space]
        [SerializeField] [Min(0f)] private float bounceSpeed = 2f;
        [SerializeField] [Min(0f)] private float minVelocity = 0.1f;

        public float BounceSpeed => bounceSpeed;

        public void TryBounce(GameObject sourceObject)
        {
            var bounceable = sourceObject.GetComponent<IBounceable>();
            if (bounceable != null)
            {
                bounceable.Bounce(bounceSpeed);
                Destroy(gameObject);
            }
        }

        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            if (vulnerableTypes.IsVulnerableTo(damageType))
            {
                Destroy(gameObject);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.attachedRigidbody.velocity.y <= -minVelocity) TryBounce(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.attachedRigidbody.velocity.y >= minVelocity) TryBounce(collision.gameObject);
        }
    }
}