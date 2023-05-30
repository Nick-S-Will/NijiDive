using System;
using UnityEngine;

namespace NijiDive.Utilities
{
    public static class ScriptingUtilities
    {
        /// <summary>
        /// Makes <paramref name="objects"/> dirty in the scene. For making script changes permanent
        /// </summary>
        /// <param name="objects">Objects to be made dirty</param>
        public static void SetDirty(params UnityEngine.Object[] objects)
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

        /// <summary>
        /// Calls <paramref name="callbackFunction"/> after all inspectors update. Workaround for when "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate" warning is seemingly useless
        /// </summary>
        /// <param name="callbackFunction">Function to call</param>
        public static void DelayCall(Action callbackFunction)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += new UnityEditor.EditorApplication.CallbackFunction(callbackFunction);
#endif
        }
    }
}