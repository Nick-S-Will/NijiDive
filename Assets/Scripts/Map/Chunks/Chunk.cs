using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Map.Chunks
{
    [CreateAssetMenu(menuName = "NijiDive/Map/Chunk")]
    public class Chunk : ScriptableObject
    {
        public TileBase[] groundTiles = new TileBase[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE], platformTiles = new TileBase[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        public EntityPosition[] entities = new EntityPosition[0];
        [Space]
        public Chunk leftChunk;
        public Chunk rightChunk;

        public static readonly Vector3Int BoundSize = new Vector3Int(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, 1);

        private void OnValidate()
        {
            var fixedArrayLength = Constants.CHUNK_SIZE * Constants.CHUNK_SIZE;
            if (groundTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(groundTiles)}'s length must be {nameof(Chunk)}.{nameof(Constants.CHUNK_SIZE)} squared.");
                Array.Resize(ref groundTiles, Constants.CHUNK_SIZE);
            }
            if (platformTiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(platformTiles)}'s length must be {nameof(Chunk)}.{nameof(Constants.CHUNK_SIZE)} squared.");
                Array.Resize(ref platformTiles, Constants.CHUNK_SIZE);
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
                return Instantiate(entity, position + positionOffset, Quaternion.identity, parent).transform;
            }
        }
    }
}