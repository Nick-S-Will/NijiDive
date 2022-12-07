using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Managers.Levels;
using NijiDive.Map.Chunks;
using NijiDive.Map;

namespace NijiDive.Managers.Map
{
    public class MapManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid entityGrid;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private bool generateAtStart = true;

        [Header("Visualizers")]
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private bool showLastDamagePoint, showNextChunkBounds;

        private Level currentLevel;
        private Tilemap[] maps;
        private Vector3 damagePoint;
        private int chunkCount;
        private bool safeChunkGenerated;

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
                Debug.LogError($"Multiple {nameof(MapManager)}s found in scene", this);
                gameObject.SetActive(false);
                return;
            }

            maps = new Tilemap[] { groundMap, platformMap };
            damagePoint = float.MaxValue * Vector3.up;
            chunkCount = 0;
        }

        private void Start()
        {
            currentLevel = LevelManager.singleton.GetCurrentLevel();
            if (currentLevel && generateAtStart) for (int i = 0; i < currentLevel.chunkCount; i++) AddRow();
        }

        #region Chunk Bounds
        private BoundsInt NextChunkBounds() => new BoundsInt(Constants.CHUNK_SIZE / 2 * Vector3Int.left + chunkCount * Constants.CHUNK_SIZE * Vector3Int.down, Chunk.BoundSize);
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
        private void AddChunk(Chunk chunk, BoundsInt chunkBounds)
        {
            groundMap.SetTilesBlock(chunkBounds, chunk.groundTiles);
            platformMap.SetTilesBlock(chunkBounds, chunk.platformTiles);
            foreach (var entity in chunk.entities)
            {
                try { _ = entity.Spawn(entityGrid.transform, chunkBounds.min); } 
                catch (System.NullReferenceException) { Debug.LogError($"Chunk \"{chunk.name}\" has a null entity."); }
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

            chunkCount++;
        }

        [ContextMenu("Generate New Row")]
        public void AddRow()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("\"Add New Chunk\" must be used in play mode.");
                return;
            }

            if (chunkCount < currentLevel.startChunks.Length) AddRow(currentLevel.startChunks[chunkCount]);
            else if (chunkCount < currentLevel.chunkCount - 1)
            {
                Chunk toSpawn;
                if (safeChunkGenerated) toSpawn = currentLevel.RandomMainChunk();
                else
                {
                    var safeThreshold = Mathf.Lerp(currentLevel.safeZoneChanceAtStart, currentLevel.safeZoneChanceAtEnd, (float)chunkCount / currentLevel.chunkCount);
                    if (Random.Range(0f, 100f) > safeThreshold)
                    {
                        toSpawn = currentLevel.RandomSafeChunk();
                        safeChunkGenerated = true;
                    }
                    else toSpawn = currentLevel.RandomMainChunk();
                }
                AddRow(toSpawn);
            }
            else AddRow(currentLevel.endChunk);
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

        public bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point)
        {
            damagePoint = point;

            var tileCell = groundMap.WorldToCell(point);
            if (groundMap.GetTile(tileCell) is IDamageable damageable && damageable.TryDamage(sourceObject, damage, damageType, point))
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