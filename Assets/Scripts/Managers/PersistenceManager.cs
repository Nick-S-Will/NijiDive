using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NijiDive.Managers.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        [SerializeField] private PersistentObject[] persistentObjects;

        private GameObject[] persistentObjectInstances;

        public static UnityEvent OnLoaded = new UnityEvent();
        private static Scene persistentScene;
        private static bool loaded;

        // Added second condition because loading script causes null array to be set to 0 length
        public bool IsShowingPersistentObjects => persistentObjectInstances != null && persistentObjectInstances.Length > 0;

        private void Awake()
        {
            if (!loaded)
            {
                if (IsShowingPersistentObjects) HideObjects();

                var gameObjects = SpawnPersistentObjects();
                foreach (var gameObject in gameObjects) DontDestroyOnLoad(gameObject);
                if (gameObjects.Length > 0) persistentScene = gameObjects[0].scene;

                loaded = true;
            }

            OnLoaded?.Invoke();
            Destroy(gameObject);
        }

        private GameObject[] SpawnPersistentObjects(Func<GameObject, UnityEngine.Object> instantiate)
        {
            var instances = new List<GameObject>();

            foreach (var persistentObject in persistentObjects)
            {
                if (!persistentObject.enabled) continue;

                instances.Add((GameObject)instantiate(persistentObject.gameObject));
                instances[instances.Count - 1].transform.SetSiblingIndex(persistentObject.transformIndex);
            }

            return instances.ToArray();
        }

        private GameObject[] SpawnPersistentObjects() => SpawnPersistentObjects(Instantiate);

#if UNITY_EDITOR
        private GameObject[] SpawnPersistentObjectsAsPrefabs() => SpawnPersistentObjects(PrefabUtility.InstantiatePrefab);

        public void ShowObjects()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Persistent objects are only meant to be shown during testing");
                return;
            }
            if (IsShowingPersistentObjects)
            {
                Debug.LogWarning("Persistent objects are already showing");
                return;
            }
            if (persistentObjects.Length == 0)
            {
                Debug.LogWarning($"{nameof(persistentObjects)} array is empty");
                return;
            }

            persistentObjectInstances = SpawnPersistentObjectsAsPrefabs();
        }

        public void HideObjects()
        {
            if (!IsShowingPersistentObjects)
            {
                Debug.LogWarning("No persistent objects to hide");
                return;
            }

            if (Application.isPlaying)
            {
                Debug.LogError("Persistent objects can't be showing in play mode");
                Application.Quit();
            }
            else
            {
                foreach (var gameObject in persistentObjectInstances) DestroyImmediate(gameObject);
                persistentObjectInstances = null;
            }
        }
#endif

        public static T FindPersistentObjectOfType<T>()
        {
            foreach (var gameObject in persistentScene.GetRootGameObjects())
            {
                T obj = gameObject.GetComponentInChildren<T>();
                if (obj != null) return obj;
            }

            return default;
        }

        [Serializable]
        private class PersistentObject
        {
            public GameObject gameObject;
            [Min(0)] public int transformIndex;
            /// <summary>
            /// Determines if this <see cref="PersistentObject"/> is spawned during testing
            /// </summary>
            public bool enabled = true;
        }
    }
}