using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Map.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BreakableTile")]
    public class BreakableTile : BaseTile, IDamageable
    {
        // Vector2 is the position of the break
        public UnityEvent<Vector2> OnBreak;
        [Space]
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;

        public DamageType VulnerableTypes => vulnerableTypes;

        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            if (vulnerableTypes.IsVulnerableTo(damageType))
            {
                OnBreak?.Invoke(point);
                return true;
            }

            return false;
        }
    }
}