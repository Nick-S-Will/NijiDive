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

        private Collider2D coinCollider, targetCollider;

        private void Start()
        {
            coinCollider = GetComponent<Collider2D>();
            targetCollider = MobManager.singleton.Target.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(coinCollider, targetCollider);

            OnCollect.AddListener(SelfDestruct);
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, targetCollider.transform.position) < coinCollider.bounds.extents.magnitude)
            {
                CoinManager.singleton.CollectCoin(value);
                OnCollect?.Invoke();
            }
        }

        private void SelfDestruct()
        {
            Destroy(gameObject);
        }
    }
}