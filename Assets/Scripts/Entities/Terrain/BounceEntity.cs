using UnityEngine;

namespace NijiDive.Entities.Terrain
{
    public class BounceEntity : Entity, IDamageable
    {
        [Space]
        [SerializeField] [Min(0f)] private float bounceSpeed = 2f;
        [SerializeField] [Min(0f)] private float minVelocity = 0.1f;

        public float BounceSpeed => bounceSpeed;

        private static DamageType vulnerableTypes = DamageType.Player | DamageType.Stomp | DamageType.Projectile;

        private bool IsFallingFastEnough(MonoBehaviour monoBehaviour)
        {
            var rigidBody = monoBehaviour.GetComponent<Rigidbody2D>();
            return rigidBody && rigidBody.velocity.y <= -minVelocity;
        } 

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
                if (damageType.ContainsAny(DamageType.Stomp) && IsFallingFastEnough(sourceBehaviour)) TryBounce(sourceBehaviour.gameObject);
                else if (damageType.ContainsAny(DamageType.Projectile)) Destroy(gameObject);
                return true;
            }

            return false;
        }
    }
}