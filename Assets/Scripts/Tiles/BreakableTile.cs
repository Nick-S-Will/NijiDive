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
        public UnityEvent OnBreak;

        public DamageType VulnerableTypes => vulnerableTypes;
    }
}