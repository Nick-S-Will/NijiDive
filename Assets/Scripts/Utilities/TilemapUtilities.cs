using UnityEngine;

namespace NijiDive.Utilities
{
    public static class TilemapUtilities
    {
        /// <summary>
        /// Finds prefab placed in <paramref name="cell"/> relative to <paramref name="grid"/>
        /// </summary>
        /// <returns>Object at given cell if one is found else null</returns>
        public static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int cell)
        {
            var min = grid.LocalToWorld(grid.CellToLocalInterpolated(cell));
            var max = grid.LocalToWorld(grid.CellToLocalInterpolated(cell + Vector3Int.one));
            var bounds = new Bounds((min + max) / 2f, max - min);

            foreach (Transform child in parent)
            {
                if (bounds.Contains(child.position)) return child;
            }

            return null;
        }
    }
}