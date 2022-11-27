using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

namespace NijiDive.Map.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BreakableTile")]
    public class BreakableTile : Tile, IDamageable
    {
        ///<see cref="GameObject"/> is the source object which broke this tile
        public UnityEvent<GameObject> OnBreak;
        [Space]
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;

        public DamageType VulnerableTypes => vulnerableTypes;

        public bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point)
        {
            if (vulnerableTypes.IsVulnerableTo(damageType))
            {
                OnBreak?.Invoke(sourceObject);
                return true;
            }

            return false;
        }
    }
}