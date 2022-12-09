using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Mobs;
using NijiDive.Managers.Coins;

namespace NijiDive.Entities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Coin : Entity
    {
        public UnityEvent OnCollect;
        [Space]
        [SerializeField] private CoinValue value = CoinValue.Small;

        private Rigidbody2D body2D;
        private Collider2D coinCollider;
        private Transform target;

        private void Start()
        {
            body2D = GetComponent<Rigidbody2D>();
            coinCollider = GetComponent<Collider2D>();
            target = MobManager.singleton.Target;
            Physics2D.IgnoreCollision(coinCollider, target.GetComponent<Collider2D>());

            OnCollect.AddListener(SelfDestruct);
        }

        private void FixedUpdate()
        {
            var maxCollectDistance = body2D.velocity.magnitude * Time.fixedDeltaTime + coinCollider.bounds.extents.magnitude;
            if (Vector3.Distance(transform.position, target.position) < maxCollectDistance)
            {
                CoinManager.singleton.CollectCoin(value);
                OnCollect?.Invoke();
            }
        }

        public override void Pause(bool paused)
        {
            if (IsPaused == paused || this == null) return;

            enabled = !paused;
            body2D.simulated = enabled;
            var animator = GetComponentInChildren<Animator>();
            if (animator) animator.enabled = enabled;
            IsPaused = paused;
        }

        private void SelfDestruct()
        {
            Destroy(gameObject);
        }
    }
}