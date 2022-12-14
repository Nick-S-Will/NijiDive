using NijiDive.Managers.Coins;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NijiDive.Map.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/CoinTile")]
    public class CoinTile : BreakableTile
    {
        [Space]
        [SerializeField] private BreakableTile baseTile;
        [SerializeField] [Range(0f, 1f)] private float coinTileChance = 0.05f;
        [SerializeField] [Min(1)] private int coinCount = 6;

        public override void Setup(Tilemap tilemap, Vector3Int cell)
        {
            base.Setup(tilemap, cell);

            // Removes since setup is called for every instance of this tile, but the event is part of the prefab
            OnBreak.RemoveListener(TrySpawnCoins);
            if (Random.Range(0f, 1f) <= coinTileChance) OnBreak.AddListener(TrySpawnCoins);
            else tilemap.SetTile(cell, baseTile);
        }

        public void TrySpawnCoins(Vector2 breakPoint)
        {
            CoinManager.singleton.ParseAndSpawnCoinSizes(breakPoint, coinCount);
        }
    }
}