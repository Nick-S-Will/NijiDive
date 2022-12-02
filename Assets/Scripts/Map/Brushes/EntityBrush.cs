using UnityEditor.Tilemaps;
using UnityEngine;

#if UNITY_EDITOR
namespace NijiDive.Map.Brushes
{
    [CreateAssetMenu(fileName = "New Entity Brush", menuName = "NijiDive/Brushes/Entity Brush")]
    [CustomGridBrush(false, true, false, "New Entity Brush")]
    public class EntityBrush : GameObjectBrush
    {
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var erased = GetObjectInCell(gridLayout, brushTarget.transform, position);
            if (erased)
            {
                UnityEditor.Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }

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
#endif