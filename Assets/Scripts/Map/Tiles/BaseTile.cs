using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Map.Tiles
{
    public abstract class BaseTile : Tile
    {
        public virtual void Setup(Tilemap tilemap, Vector3Int cell) { }
    }
}