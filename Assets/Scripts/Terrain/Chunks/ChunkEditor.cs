using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using NijiDive.Utilities;

namespace NijiDive.Terrain.Chunks
{
    public class ChunkEditor : MonoBehaviour
    {
        [SerializeField] private Tilemap groundMap, platformMap;
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

            groundMap.SetTilesBlock(EditorBounds, toLoad.groundTiles);
            platformMap.SetTilesBlock(EditorBounds, toLoad.platformTiles);
        }

        public void SaveChunk()
        {
            var newChunk = ScriptableObject.CreateInstance<Chunk>();
            newChunk.name = newChunkFileName;

            newChunk.groundTiles = groundMap.GetTilesBlock(EditorBounds);
            newChunk.platformTiles = platformMap.GetTilesBlock(EditorBounds);

            ScriptableObjectUtilities.ForceSaveChunkAsset(newChunk);
        }

        public void ClearEditor()
        {
            groundMap.ClearAllTiles();
            platformMap.ClearAllTiles();
        }
    }
}