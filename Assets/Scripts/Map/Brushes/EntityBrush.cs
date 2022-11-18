using UnityEditor.Tilemaps;
using UnityEngine;

using NijiDive.Utilities;

#if UNITY_EDITOR
namespace NijiDive.Map.Brushes
{
    [CreateAssetMenu(fileName = "New Entity Brush", menuName = "NijiDive/Brushes/Entity Brush")]
    [CustomGridBrush(false, true, false, "New Entity Brush")]
    public class EntityBrush : GameObjectBrush
    {
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var erased = TilemapUtilities.GetObjectInCell(gridLayout, brushTarget.transform, position);
            if (erased)
            {
                UnityEditor.Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }
    }
}
#endif