using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

namespace NijiDive.Map.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BreakableTile")]
    public class BreakableTile : Tile, IDamageable
    {
        ///<see cref="GameObject"/> is the source object which broke this tile
        public UnityEvent<MonoBehaviour> OnBreak;
        [Space]
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;

        public DamageType VulnerableTypes => vulnerableTypes;

        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            if (vulnerableTypes.IsVulnerableTo(damageType))
            {
                OnBreak?.Invoke(sourceBehaviour);
                return true;
            }

            return false;
        }
    }
}