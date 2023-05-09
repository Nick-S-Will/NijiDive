using UnityEngine;

namespace NijiDive.Utilities
{
    public static class ScriptingUtilities
    {
        public static void SetDirty(params Object[] objects)
        {
            #if UNITY_EDITOR
            foreach (var obj in objects) UnityEditor.EditorUtility.SetDirty(obj);
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
            #endif
        }
    }
}