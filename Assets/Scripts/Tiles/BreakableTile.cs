using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

namespace NijiDive.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BreakableTile")]
    public class BreakableTile : Tile
    {
        [Space]
        public UnityEvent OnBreak;
    }
}