using UnityEngine;
using UnityEditor;

using NijiDive.Managers.Persistence;

namespace NijiDive.Editors
{
    [CustomEditor(typeof(PersistenceManager))]
    public class PersistenceManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var persistenceManager = (PersistenceManager)target;

            if (persistenceManager.IsShowingPersistentObjects)
            {
                if (GUILayout.Button("Hide Objects")) persistenceManager.HideObjects();
            }
            else
            {
                if (GUILayout.Button("Show Objects")) persistenceManager.ShowObjects();
            }
        }
    }
}