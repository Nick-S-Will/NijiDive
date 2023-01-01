using UnityEngine;

using NijiDive.Map.Chunks;

namespace NijiDive.Map
{
    [CreateAssetMenu(menuName = "NijiDive/Map/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private Chunk[] startChunks, shopJunctionChunks, safeZoneChunks, mainChunks;
        [SerializeField] private Chunk endChunk;
        [Tooltip("Number of chunks generated in level. Set to 1 for start level")]
        [SerializeField] private int rowCount = 15;
        [Space]
        [SerializeField] [Min(1)] private int safeZoneRowInterval = 3;
        [SerializeField] [Range(0f, 1f)] private float shopChance = 0.5f;

        public Chunk[] StartChunks => startChunks;
        public Chunk RandomShopJuntionChunk() => shopJunctionChunks[Random.Range(0, shopJunctionChunks.Length)];
        public Chunk RandomSafeZoneChunk() => safeZoneChunks[Random.Range(0, safeZoneChunks.Length)];
        public Chunk RandomMainChunk() => mainChunks[Random.Range(0, mainChunks.Length)];
        public Chunk EndChunk => endChunk;
        public int RowCount => rowCount;
        public int SafeZoneIndexInterval => safeZoneRowInterval + 1;
        public float ShopChance => shopChance;

        private void OnValidate()
        {
            if (rowCount == 1) return;

            var minChunks = startChunks.Length + 1;
            if (rowCount < minChunks)
            {
                Debug.LogWarning($"{nameof(rowCount)} must be at least 1 more than the length of {nameof(startChunks)} to ensure the {nameof(endChunk)} can generate");
                rowCount = minChunks;
            }
        }
    }
}