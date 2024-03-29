using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.PlayerBased;
using NijiDive.Managers.Coins;

namespace NijiDive.Entities.Contact
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
            target = PlayerBasedManager.Player.transform;

            OnCollect.AddListener(() => CoinManager.singleton.CollectCoin(value));
            OnCollect.AddListener(SelfDestruct);
        }

        private void FixedUpdate()
        {
            var maxCollectDistance = body2D.velocity.magnitude * Time.fixedDeltaTime + coinCollider.bounds.extents.magnitude;
            // Done this was so that coins pass through entities
            if (Vector3.Distance(transform.position, target.position) < maxCollectDistance) OnCollect?.Invoke();
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