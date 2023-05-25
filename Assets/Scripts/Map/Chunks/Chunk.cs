using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Map.Chunks
{
    [CreateAssetMenu(menuName = "NijiDive/Map/Chunk")]
    public class Chunk : ScriptableObject
    {
        public TileBase[] groundTiles = new TileBase[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE], platformTiles = new TileBase[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE], waterTiles = new TileBase[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        public EntityPosition[] entities = new EntityPosition[0];
        [Space]
        public Chunk leftChunk;
        public Chunk rightChunk;

        public static readonly Vector3Int BoundSize = new Vector3Int(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, 1);

        private void CheckTileLength(ref TileBase[] tiles)
        {
            var fixedArrayLength = Constants.CHUNK_SIZE * Constants.CHUNK_SIZE;
            if (tiles.Length != fixedArrayLength)
            {
                Debug.LogWarning($"{nameof(tiles)}'s length must be {nameof(Constants.CHUNK_SIZE)} squared.");
                Array.Resize(ref tiles, Constants.CHUNK_SIZE);
            }
        }

        private void OnValidate()
        {
            CheckTileLength(ref groundTiles);
            CheckTileLength(ref platformTiles);
            CheckTileLength(ref waterTiles);
        }

        [Serializable]
        public class EntityPosition
        {
            public GameObject entityPrefab;
            public Vector3 position;

            public EntityPosition(GameObject entity, Vector3 position)
            {
                entityPrefab = entity;
                this.position = position;
            }

            public Transform Spawn(Transform parent, Vector3 positionOffset = default)
            {
                return Instantiate(entityPrefab, position + positionOffset, Quaternion.identity, parent).transform;
            }
        }
    }
}