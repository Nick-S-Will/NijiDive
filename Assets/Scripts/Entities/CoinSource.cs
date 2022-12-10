using UnityEngine;

using NijiDive.Managers.Coins;

namespace NijiDive.Entities
{
    public class CoinSource : Entity, IDamageable
    {
        [Space]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Tooltip("Sprites used to indicate the remaining coin content. Used in reverse order")]
        [SerializeField] private Sprite[] contentSprites = new Sprite[1];
        [Space]
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;
        [SerializeField] private Vector2 coinSpawnOffset = 0.1f * Vector2.up;
        [SerializeField] private int coinsOnDamage = 4, coinsOnDeath = 14;

        private int spriteIndex;

        private void Start()
        {
            spriteIndex = contentSprites.Length - 1;
            spriteRenderer.sprite = contentSprites[spriteIndex];
        }

        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            if (!vulnerableTypes.IsVulnerableTo(damageType)) return false;

            for (int i = 0; i < damage; i++)
            {
                var isDestroyed = spriteIndex == 0;
                if (isDestroyed) Destroy(gameObject);
                else
                {
                    Flash(spriteRenderer);
                    spriteRenderer.sprite = contentSprites[--spriteIndex];
                }

                CoinManager.singleton.ParseAndSpawnCoinSizes(point + coinSpawnOffset, isDestroyed ? coinsOnDeath : coinsOnDamage);

                if (isDestroyed) break;
            }

            return true;
        }
    }
}