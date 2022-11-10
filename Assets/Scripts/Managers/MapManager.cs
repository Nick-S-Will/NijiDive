using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Map.Chunks;

namespace NijiDive.Managers.Map
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid enemyGrid;
        [SerializeField] private LayerMask groundMask;

        [Space]
        [SerializeField] private Chunk[] chunkOptions;
        [SerializeField] private int chunksInLevel = 15;
        [SerializeField] private bool generateAtStart = true;

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

        private void Awake()
        {
            maps = new Tilemap[] { groundMap, platformMap };
            damagePoint = float.MaxValue * Vector3.up;
            chunkCount = 0;

            if (chunkOptions.Length == 0) Debug.LogError($"{nameof(chunkOptions)} is empty");
            else if (generateAtStart) for (int i = 0; i < chunksInLevel; i++) AddChunk();
        }

        [ContextMenu("Add New Chunk")]
        public void AddChunk()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("\"Add New Chunk\" must be used at runtime.");
                return;
            }

            var chunk = chunkOptions[Random.Range(0, chunkOptions.Length)];
            var bounds = NextChunkBounds;

            groundMap.SetTilesBlock(bounds, chunk.groundTiles);
            platformMap.SetTilesBlock(bounds, chunk.platformTiles);
            foreach (var enemy in chunk.enemies) _ = enemy.Spawn(enemyGrid.transform, bounds.min);

            chunkCount++;
        }

        public bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point)
        {
            damagePoint = point;

            foreach (var groundMap in maps)
            {
                var tileCell = groundMap.WorldToCell(point);
                if (groundMap.GetTile(tileCell) is IDamageable damageable && damageable.TryDamage(sourceObject, damage, damageType, point))
                {
                    groundMap.SetTile(tileCell, null);
                }
            }

            return false;
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

        private void OnValidate()
        {
            if (chunkOptions.Length > 0) enabled = true;
        }
    }
}