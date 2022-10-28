using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Tiles;

namespace NijiDive.Managers.Map
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap;
        [Space]
        [SerializeField] private LayerMask groundMask;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint;

        private Vector3 damagePoint = float.MaxValue * Vector3.up;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;

        public bool TryDamage(int damage, DamageType damageType, Vector2 point)
        {
            var tileCell = groundMap.WorldToCell(point);
            damagePoint = tileCell;
            if (groundMap.GetTile(tileCell) is BreakableTile bt && bt.VulnerableTypes.IsVulnerableTo(damageType))
            {
                damagePoint = point;

                bt.OnBreak?.Invoke();
                groundMap.SetTile(tileCell, null);
                
                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (showLastDamagePoint)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawSphere(damagePoint, 0.1f);
            }
        }
    }
}