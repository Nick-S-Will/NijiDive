using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Map.Chunks;

namespace NijiDive.Managers.Map
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid entityGrid;
        [SerializeField] private LayerMask groundMask;

        [Space]
        [SerializeField] private Chunk[] chunkOptions;
        [SerializeField] private int chunksInLevel = 15;
        [SerializeField] private bool generateAtStart = true;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint, showNextChunkBounds;

        private Tilemap[] maps;
        private Vector3 damagePoint;
        private int chunkCount;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;
        private BoundsInt NextChunkBounds => new BoundsInt(GameConstants.CHUNK_SIZE / 2 * Vector3Int.left + chunkCount * GameConstants.CHUNK_SIZE * Vector3Int.down, Chunk.BoundSize);

        public static MapManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {typeof(MapManager)}s exist", this);
                gameObject.SetActive(false);
                return;
            }

            maps = new Tilemap[] { groundMap, platformMap };
            damagePoint = float.MaxValue * Vector3.up;
            chunkCount = 0;

            if (chunkOptions.Length == 0) Debug.LogError($"{nameof(chunkOptions)} is empty");
            else if (generateAtStart) for (int i = 0; i < chunksInLevel; i++) AddRow(chunkOptions[Random.Range(0, chunkOptions.Length)]);
        }

        private BoundsInt ShiftLeft(BoundsInt bounds)
        {
            bounds.position += GameConstants.CHUNK_SIZE * Vector3Int.left;
            return bounds;
        }
        private BoundsInt ShiftRight(BoundsInt bounds)
        {
            bounds.position += GameConstants.CHUNK_SIZE * Vector3Int.right;
            return bounds;
        }

        #region Level Generation
        private void AddChunk(Chunk chunk, BoundsInt chunkBounds)
        {
            groundMap.SetTilesBlock(chunkBounds, chunk.groundTiles);
            platformMap.SetTilesBlock(chunkBounds, chunk.platformTiles);
            foreach (var entity in chunk.entities) _ = entity.Spawn(entityGrid.transform, chunkBounds.min);
        }

        private void AddSideChunks(Chunk baseChunk, BoundsInt bounds, bool right)
        {
            var chunk = right ? baseChunk.rightChunk : baseChunk.leftChunk;
            while (chunk != null)
            {
                bounds = right ? ShiftRight(bounds) : ShiftLeft(bounds);
                AddChunk(chunk, bounds);
                chunk = right ? chunk.rightChunk : chunk.leftChunk;
            }
        }

        [ContextMenu("Add New Chunk")]
        public void AddRow(Chunk baseChunk)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("\"Add New Chunk\" must be used in play mode.");
                return;
            }

            var chunkBounds = NextChunkBounds;
            AddChunk(baseChunk, chunkBounds);
            AddSideChunks(baseChunk, chunkBounds, false);
            AddSideChunks(baseChunk, chunkBounds, true);

            chunkCount++;
        }
        #endregion

        public bool PointInCenter(Vector3 point)
        {
            return Mathf.Abs(point.x - entityGrid.transform.position.x) < GameConstants.CHUNK_SIZE / 2f;
        }

        public TileBase GetTile(Vector2 point)
        {
            foreach (var groundMap in maps)
            {
                var tileCell = groundMap.WorldToCell(point);
                var tile = groundMap.GetTile(tileCell);

                if (tile != null) return tile;
            }

            return null;
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
            if (showNextChunkBounds)
            {
                var bounds = NextChunkBounds;
                Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
            }
        }

        private void OnValidate()
        {
            if (chunkOptions.Length > 0) enabled = true;
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}