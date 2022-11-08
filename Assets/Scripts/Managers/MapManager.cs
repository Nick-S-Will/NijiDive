using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Terrain.Chunks;
using NijiDive.Terrain.Tiles;

namespace NijiDive.Managers.Map
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private LayerMask groundMask;

        [Space]
        [SerializeField] private Chunk[] chunkOptions;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint, showChunkBounds;

        private Tilemap[] maps;
        private Vector3 damagePoint;
        private int chunkCount;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;
        private BoundsInt NextChunkBounds => new BoundsInt(Chunk.SIZE / 2 * Vector3Int.left + chunkCount * Chunk.SIZE * Vector3Int.down, Chunk.BoundSize);

        private void Start()
        {
            maps = new Tilemap[] { groundMap, platformMap };
            damagePoint = float.MaxValue * Vector3.up;
            chunkCount = 0;

            if (chunkOptions.Length == 0) Debug.LogError($"{nameof(chunkOptions)} is empty");
            else AddChunk();
            AddChunk();
        }

        public void AddChunk()
        {
            var chunk = chunkOptions[Random.Range(0, chunkOptions.Length)];
            var bounds = NextChunkBounds;

            groundMap.SetTilesBlock(bounds, chunk.groundTiles);
            platformMap.SetTilesBlock(bounds, chunk.platformTiles);

            chunkCount++;
        }

        public bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point)
        {
            foreach (var groundMap in maps) {
                var tileCell = groundMap.WorldToCell(point);
                print(tileCell);
                damagePoint = tileCell;
                if (groundMap.GetTile(tileCell) is BreakableTile bt && bt.VulnerableTypes.IsVulnerableTo(damageType))
                {
                    damagePoint = point;

                    bt.OnBreak?.Invoke(sourceObject);
                    groundMap.SetTile(tileCell, null);
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            if (showLastDamagePoint)
            {
                Gizmos.DrawSphere(damagePoint, 0.1f);
            }
            if (showChunkBounds)
            {
                var bounds = NextChunkBounds;
                Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
            }
        }
    }
}