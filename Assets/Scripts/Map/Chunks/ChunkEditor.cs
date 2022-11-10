#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using NijiDive.Utilities;
using NijiDive.Controls.Enemies;

namespace NijiDive.Map.Chunks
{
    public class ChunkEditor : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid enemyGrid;
        [Space]
        [SerializeField] private Chunk toLoad;
        [Space]
        [SerializeField] private string newChunkFileName = "New Chunk";

        private static readonly BoundsInt EditorBounds = new BoundsInt(new Vector3Int(0, 1 - Chunk.SIZE, 0), Chunk.BoundSize);

        public void LoadChunk()
        {
            if (toLoad == null)
            {
                Debug.LogWarning($"{nameof(toLoad)} is not assigned.");
                return;
            }

            ClearEnemies();

            groundMap.SetTilesBlock(EditorBounds, toLoad.groundTiles);
            platformMap.SetTilesBlock(EditorBounds, toLoad.platformTiles);
            foreach (var enemyPosition in toLoad.enemies) _ = enemyPosition.Spawn(enemyGrid.transform);
        }

        public void SaveChunk()
        {
            var newChunk = ScriptableObject.CreateInstance<Chunk>();
            newChunk.name = newChunkFileName;
            newChunk.groundTiles = groundMap.GetTilesBlock(EditorBounds);
            newChunk.platformTiles = platformMap.GetTilesBlock(EditorBounds);
            newChunk.enemies = GetEnemyPositions();

            ScriptableObjectUtilities.ForceSaveChunkAsset(newChunk);
        }

        public void ClearEditor()
        {
            groundMap.ClearAllTiles();
            platformMap.ClearAllTiles();
            ClearEnemies();
        }

        private EnemyPosition[] GetEnemyPositions()
        {
            var enemies = new List<EnemyPosition>();
            foreach (Transform child in enemyGrid.transform)
            {
                var enemy = PrefabUtility.GetCorrespondingObjectFromSource(child.GetComponent<Enemy>());
                if (enemy == null) continue;
                enemies.Add(new EnemyPosition(enemy, child.position - EditorBounds.min));
            }

            return enemies.ToArray();
        }

        private void ClearEnemies()
        {
            foreach (Transform child in enemyGrid.transform.Cast<Transform>().ToList()) DestroyImmediate(child.gameObject);
        }
    }
}
#endif