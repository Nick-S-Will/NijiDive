#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using System.IO;

namespace NijiDive.Map.Chunks
{
    public class ChunkEditor : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap, platformMap;
        [SerializeField] private Grid entityGrid;
        [Space]
        [SerializeField] private Chunk toLoad;
        [Space]
        public string newChunkFileName = "New Chunk";

        private static readonly BoundsInt EditorBounds = new BoundsInt(new Vector3Int(0, 1 - Constants.CHUNK_SIZE, 0), Chunk.BoundSize);

        public void LoadChunk()
        {
            if (toLoad == null)
            {
                Debug.LogWarning($"{nameof(toLoad)} is not assigned.");
                return;
            }

            ClearEntities();

            groundMap.SetTilesBlock(EditorBounds, toLoad.groundTiles);
            platformMap.SetTilesBlock(EditorBounds, toLoad.platformTiles);
            foreach (var entityPosition in toLoad.entities)
            {
                var entity = (GameObject)PrefabUtility.InstantiatePrefab(entityPosition.entity, entityGrid.transform);
                entity.transform.position = entityPosition.position + EditorBounds.min;
            }
        }

        public static bool SaveChunkAsset(Chunk newChunk, string nameSuffix = "")
        {
            var path = $"Assets/Prefabs/Chunks/{newChunk.name + nameSuffix}.asset";

            if (File.Exists(path)) return false;

            AssetDatabase.CreateAsset(newChunk, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }

        public static void ForceSaveChunkAsset(Chunk newChunk)
        {
            if (SaveChunkAsset(newChunk)) return;

            var count = 1;
            while (!SaveChunkAsset(newChunk, nameSuffix: $" ({count})")) count++;
        }

        public void SaveChunk()
        {
            if (newChunkFileName == string.Empty)
            {
                Debug.LogError($"{nameof(newChunkFileName)} must be assigned to save chunk", this);
                return;
            }

            var newChunk = ScriptableObject.CreateInstance<Chunk>();
            newChunk.name = newChunkFileName;
            newChunk.groundTiles = groundMap.GetTilesBlock(EditorBounds);
            newChunk.platformTiles = platformMap.GetTilesBlock(EditorBounds);
            newChunk.entities = GetEntityPositions();

            ForceSaveChunkAsset(newChunk);
        }

        public void ClearEditor()
        {
            groundMap.ClearAllTiles();
            platformMap.ClearAllTiles();
            ClearEntities();
        }

        private Chunk.EntityPosition[] GetEntityPositions()
        {
            var entities = new List<Chunk.EntityPosition>();
            foreach (Transform child in entityGrid.transform)
            {
                var entityPrefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (entityPrefab == null) continue;
                entities.Add(new Chunk.EntityPosition(entityPrefab, child.position - EditorBounds.min));
            }

            return entities.ToArray();
        }

        private void ClearEntities()
        {
            foreach (Transform child in entityGrid.transform.Cast<Transform>().ToList()) DestroyImmediate(child.gameObject);
        }
    }
}
#endif