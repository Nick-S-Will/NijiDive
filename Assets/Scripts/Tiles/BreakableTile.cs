using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

namespace NijiDive.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BreakableTile")]
    public class BreakableTile : Tile
    {
        [SerializeField] private DamageType vulnerableTypes = DamageType.Player | DamageType.Projectile;
        [Space]
        ///<see cref="GameObject"/> is the source object which broke this tile
        public UnityEvent<GameObject> OnBreak;

        public DamageType VulnerableTypes => vulnerableTypes;
    }
}