using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Map.Chunks
{
    [Serializable]
    public class Chunk : ScriptableObject
    {
        public TileBase[] groundTiles = new TileBase[GameConstants.CHUNK_SIZE * GameConstants.CHUNK_SIZE], platformTiles = new TileBase[GameConstants.CHUNK_SIZE * GameConstants.CHUNK_SIZE];
        public EntityPosition[] entities;
        [Space]
        public Chunk leftChunk;
        public Chunk rightChunk;

        public static readonly Vector3Int BoundSize = new Vector3Int(GameConstants.CHUNK_SIZE, GameConstants.CHUNK_SIZE, 1);


        private void OnValidate()
        {
            var fixedArrayLength = GameConstants.CHUNK_SIZE * GameConstants.CHUNK_SIZE;
            if (groundTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(groundTiles)}'s length must be {nameof(Chunk)}.{nameof(GameConstants.CHUNK_SIZE)} squared.");
                Array.Resize(ref groundTiles, GameConstants.CHUNK_SIZE);
            }
            if (platformTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(platformTiles)}'s length must be {nameof(Chunk)}.{nameof(GameConstants.CHUNK_SIZE)} squared.");
                Array.Resize(ref platformTiles, GameConstants.CHUNK_SIZE);
            }
        }
    }

    [Serializable]
    public class EntityPosition
    {
        public GameObject entity;
        public Vector3 position;

        public EntityPosition(GameObject entity, Vector3 position)
        {
            this.entity = entity;
            this.position = position;
        }

        public Transform Spawn(Transform parent, Vector3 positionOffset = default)
        {
            return UnityEngine.Object.Instantiate(entity, position + positionOffset, Quaternion.identity, parent).transform;
        }
    }
}