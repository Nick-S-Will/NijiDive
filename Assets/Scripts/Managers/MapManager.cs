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

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint;

        private Vector3 damagePoint = float.MaxValue * Vector3.up;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;

        public bool TakeDamage(int damage, DamageType damageType, Vector2 point)
        {
            var tileCell = groundMap.WorldToCell(point);
            if (groundMap.GetTile(tileCell) is BreakableTile bt)
            {
                if (!bt.VulnerableTypes.IsVulnerableTo(damageType)) return false;
                damagePoint = point;

                bt.OnBreak?.Invoke();
                groundMap.SetTile(tileCell, null);
            }

            return true;
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