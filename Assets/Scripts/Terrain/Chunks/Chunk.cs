using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Terrain.Chunks
{
    [Serializable]
    public class Chunk : ScriptableObject
    {
        public TileBase[] groundTiles = new TileBase[SIZE * SIZE], platformTiles = new TileBase[SIZE * SIZE];

        /// <summary>
        /// Width and height of chunks in NijiDive
        /// </summary>
        public const int SIZE = 13;
        public static readonly Vector3Int BoundSize = new Vector3Int(SIZE, SIZE, 1);

        private void OnValidate()
        {
            var fixedArrayLength = SIZE * SIZE;
            if (groundTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(groundTiles)}'s length must be {nameof(Chunk)}.{nameof(SIZE)} squared.");
                Array.Resize(ref groundTiles, SIZE);
            }
            if (platformTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(platformTiles)}'s length must be {nameof(Chunk)}.{nameof(SIZE)} squared.");
                Array.Resize(ref platformTiles, SIZE);
            }
        }
    }
}