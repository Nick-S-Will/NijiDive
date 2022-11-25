using UnityEngine;

using NijiDive.Map.Chunks;

namespace NijiDive.Map
{
    [CreateAssetMenu(menuName = "NijiDive/Map/Level")]
    public class Level : ScriptableObject
    {
        public Chunk[] startChunks, safeZoneChunks, mainChunks;
        public Chunk endChunk;
        public int chunkCount = 15;
        [Space]
        [Range(0f, 1f)] public float safeZoneChanceAtStart = 0.05f;
        [Range(0f, 1f)] public float safeZoneChanceAtEnd = 0.1f;

        public Chunk RandomSafeChunk() => safeZoneChunks[Random.Range(0, safeZoneChunks.Length)];
        public Chunk RandomMainChunk() => mainChunks[Random.Range(0, mainChunks.Length)];

        private void OnValidate()
        {
            var minChunks = startChunks.Length + 1;
            if (chunkCount < minChunks)
            {
                Debug.LogWarning($"{nameof(chunkCount)} must be at least 1 more than {nameof(startChunks.Length)} to ensure the {nameof(endChunk)} can generate");
                chunkCount = minChunks;
            }
        }
    }
}