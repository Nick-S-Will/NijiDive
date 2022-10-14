using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Tiles;

namespace NijiDive.Managers
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap;
        [Space]
        [SerializeField] private LayerMask groundMask;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;

        public bool TakeDamage(int damage, IDamageable.DamageType damageType, Vector2 point)
        {
            if (damageType != IDamageable.DamageType.Player) return false;

            var tileCell = groundMap.WorldToCell(point);
            if (groundMap.GetTile(tileCell) is BreakableTile bt)
            {
                bt.OnBreak?.Invoke();
                groundMap.SetTile(tileCell, null);
            }
            
            return true;
        }
    }
}