using UnityEditor.Tilemaps;
using UnityEngine;

using NijiDive.Utilities;

namespace NijiDive.Map.Brushes
{
    [CreateAssetMenu(fileName = "New Mob Brush", menuName = "NijiDive/Brushes/Mob Brush")]
    [CustomGridBrush(false, true, false, "New Mob Brush")]
    public class MobBrush : GameObjectBrush
    {
#if UNITY_EDITOR
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            var erased = TilemapUtilities.GetObjectInCell(gridLayout, brushTarget.transform, position);
            if (erased)
            {
                UnityEditor.Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }
#endif
    }
}