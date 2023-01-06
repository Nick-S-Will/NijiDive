using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Managers.Levels;
using NijiDive.Map.Chunks;
using NijiDive.Map;
using NijiDive.Map.Tiles;

namespace NijiDive.Managers.Map
{
    public class MapManager : Manager, IDamageable
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid entityGrid;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private bool generateAtStart = true;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint, showNextChunkBounds;

        private Tilemap[] maps;
        private Level currentLevel;
        private Vector3 damagePoint = float.MaxValue * Vector3.up;
        private int rowCount, shopIndex;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;

        public static MapManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(MapManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }

            maps = new Tilemap[] { groundMap, platformMap };
            rowCount = 0;
        }

        private void Start()
        {
            currentLevel = LevelManager.singleton ? LevelManager.singleton.GetCurrentLevel() : null;
            if (currentLevel == null) return;

            shopIndex = GenerateShopIndex();
            if (generateAtStart) for (int i = 0; i < currentLevel.RowCount; i++) AddRow();
        }

        public override void Retry() { }

        #region Chunk Bounds
        private BoundsInt NextChunkBounds() => new BoundsInt(Constants.CHUNK_SIZE / 2 * Vector3Int.left + rowCount * Constants.CHUNK_SIZE * Vector3Int.down, Chunk.BoundSize);
        private BoundsInt ShiftLeft(BoundsInt bounds)
        {
            bounds.position += Constants.CHUNK_SIZE * Vector3Int.left;
            return bounds;
        }
        private BoundsInt ShiftRight(BoundsInt bounds)
        {
            bounds.position += Constants.CHUNK_SIZE * Vector3Int.right;
            return bounds;
        }
        #endregion

        #region Level Generation
        private int GenerateShopIndex()
        {
            if (Random.Range(0f, 1f) <= currentLevel.ShopChance)
            {
                var safeZoneCount = Mathf.CeilToInt((float)currentLevel.RowCount / currentLevel.SafeZoneIndexInterval);
                return Random.Range(1, safeZoneCount) * currentLevel.SafeZoneIndexInterval;
            }
            else return -1;
        }

        private void AddChunk(Chunk chunk, BoundsInt chunkBounds)
        {
            groundMap.SetTilesBlock(chunkBounds, chunk.groundTiles);
            platformMap.SetTilesBlock(chunkBounds, chunk.platformTiles);
            foreach (var entity in chunk.entities)
            {
                try { _ = entity.Spawn(entityGrid.transform, chunkBounds.min); }
                catch (System.NullReferenceException) { Debug.LogWarning($"Chunk \"{chunk.name}\" has a null entity."); }
            }

            foreach (var map in maps)
            {
                foreach (var cell in chunkBounds.allPositionsWithin)
                {
                    var tile = map.GetTile<BaseTile>(cell);
                    if (tile) tile.Setup(map, cell);
                }
            }
        }

        private void AddSideChunks(Chunk baseChunk, BoundsInt baseBounds)
        {
            for (int i = 0; i < 2; i++)
            {
                var rightSide = i == 1;
                var chunk = rightSide ? baseChunk.rightChunk : baseChunk.leftChunk;
                var bounds = baseBounds;
                while (chunk != null)
                {
                    bounds = rightSide ? ShiftRight(bounds) : ShiftLeft(bounds);
                    AddChunk(chunk, bounds);
                    chunk = rightSide ? chunk.rightChunk : chunk.leftChunk;
                }
            }
        }

        public void AddRow(Chunk baseChunk)
        {
            var chunkBounds = NextChunkBounds();
            AddChunk(baseChunk, chunkBounds);
            AddSideChunks(baseChunk, chunkBounds);

            rowCount++;
        }

        [ContextMenu("Generate New Row")]
        public void AddRow()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("\"Add New Chunk\" must be used in play mode.");
                return;
            }

            Chunk toSpawn;
            if (rowCount < currentLevel.StartChunks.Length) toSpawn = currentLevel.StartChunks[rowCount];
            else if (rowCount < currentLevel.RowCount - 1)
            {
                if (rowCount > 0 && rowCount % currentLevel.SafeZoneIndexInterval == 0) toSpawn = rowCount == shopIndex ? currentLevel.RandomShopJuntionChunk() : currentLevel.RandomSafeZoneChunk();
                else toSpawn = currentLevel.RandomMainChunk();
            }
            else toSpawn = currentLevel.EndChunk;

            AddRow(toSpawn);
        }
        #endregion

        public bool PointInCenter(Vector3 point)
        {
            return Mathf.Abs(point.x - entityGrid.transform.position.x) < Constants.CHUNK_SIZE / 2f;
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

        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point)
        {
            damagePoint = point;

            var tileCell = groundMap.WorldToCell(point);
            if (groundMap.GetTile(tileCell) is IDamageable damageable && damageable.TryDamage(sourceBehaviour, damage, damageType, point))
            {
                groundMap.SetTile(tileCell, null);
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
                var bounds = NextChunkBounds();
                Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}