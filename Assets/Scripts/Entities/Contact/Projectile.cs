using UnityEngine;

using NijiDive.Controls.Attacks;

namespace NijiDive.Entities.Contact
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : Entity
    {
        [SerializeField] [Min(0f)] private float lifeTime = 0.2f;
        [SerializeField] [Min(1)] private int damage = 1;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private DamageType damageType = DamageType.Player | DamageType.Projectile;

        // TODO: remove weapon controller reference and make weapon controller dependant on its projectiles instead
        private WeaponController sourceWeaponController;
        private Rigidbody2D body2D;
        
        private void Start()
        {
            body2D = GetComponent<Rigidbody2D>();

            Destroy(gameObject, lifeTime);
        }

        private void FixedUpdate()
        {
            var hitInfo = Physics2D.Raycast(transform.position, -transform.up, 2f * body2D.velocity.y * Time.fixedDeltaTime, targetLayers);
            if (hitInfo.collider && hitInfo.collider != sourceWeaponController.MobCollider2D)
            {
                _ = sourceWeaponController.TryDamageCollider(this, hitInfo.collider, damageType, damage, transform.position);
                
                Destroy(gameObject);
            }
        }

        public void Setup(WeaponController source, float magnitude)
        {
            sourceWeaponController = source;

            if (body2D == null) body2D = GetComponent<Rigidbody2D>();
            body2D.velocity = magnitude * -transform.up;
        }

        public override void Pause(bool paused)
        {
            if (IsPaused == paused || this == null) return;

            body2D.simulated = !paused;
            IsPaused = paused;
        }
    }
}