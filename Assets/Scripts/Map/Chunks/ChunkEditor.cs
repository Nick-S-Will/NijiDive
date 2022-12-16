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

        private static readonly BoundsInt EditorBounds = new BoundsInt(Vector3Int.zero, Chunk.BoundSize);

        public Chunk ToLoad => toLoad;

        public void LoadChunk()
        {
            if (toLoad == null)
            {
                Debug.LogWarning($"{nameof(toLoad)} is not assigned.");
                return;
            }

            ClearEntities();

            Undo.RecordObjects(new Object[] { groundMap, platformMap }, "Chunk Editor Tile Clear");
            groundMap.SetTilesBlock(EditorBounds, toLoad.groundTiles);
            platformMap.SetTilesBlock(EditorBounds, toLoad.platformTiles);
            CreateEntities();
        }

        public void OverwriteChunk()
        {
            if (toLoad == null)
            {
                Debug.LogError($"{nameof(ToLoad)} must be assigned to overwrite", this);
                return;
            }

            Undo.RecordObject(toLoad, "Chunk Editor Overwrite");
            toLoad.groundTiles = groundMap.GetTilesBlock(EditorBounds);
            toLoad.platformTiles = platformMap.GetTilesBlock(EditorBounds);
            toLoad.entities = GetEntityPositions();
            EditorUtility.SetDirty(toLoad);
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

        public static void SaveChunkAssetSafe(Chunk newChunk)
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

            SaveChunkAssetSafe(newChunk);
        }

        // Based on code from https://answers.unity.com/questions/1587818/how-to-undo-a-lot-of-created-objects-at-once-2.html
        private void CreateEntities()
        {
            int undoID = Undo.GetCurrentGroup();

            foreach (var entityPosition in toLoad.entities)
            {
                if (entityPosition.entityPrefab == null)
                {
                    Debug.LogWarning($"Chunk \"{toLoad.name}\" has a null entity.");
                    continue;
                }

                var entity = (GameObject)PrefabUtility.InstantiatePrefab(entityPosition.entityPrefab, entityGrid.transform);
                entity.transform.position = entityPosition.position + EditorBounds.min;
                Undo.RegisterCreatedObjectUndo(entity, "Entity Spawn");
                Undo.CollapseUndoOperations(undoID);
            }
        }
        private void ClearEntities()
        {
            int undoID = Undo.GetCurrentGroup();

            foreach (Transform child in entityGrid.transform.Cast<Transform>().ToList())
            {
                Undo.DestroyObjectImmediate(child.gameObject);
                Undo.CollapseUndoOperations(undoID);
            }
        }

        public void ClearEditor()
        {
            Undo.RecordObjects(new Object[] { groundMap, platformMap }, "Chunk Editor Tile Clear");

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
    }
}
#endif