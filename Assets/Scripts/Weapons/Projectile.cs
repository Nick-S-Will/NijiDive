using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private IDamageable.DamageType damageType = IDamageable.DamageType.Player;

        public UnityEvent OnHit;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        public void SetSpeed(float magnitude)
        {
            GetComponent<Rigidbody2D>().velocity = magnitude * -transform.up;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.TakeDamage(damage, damageType))
            {
                OnHit?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}