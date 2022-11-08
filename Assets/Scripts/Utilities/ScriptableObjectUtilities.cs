#if UNITY_EDITOR
using System.IO;
using UnityEditor;

using NijiDive.Terrain.Chunks;

namespace NijiDive.Utilities
{

    public static class ScriptableObjectUtilities
    {
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
    }
}
#endif