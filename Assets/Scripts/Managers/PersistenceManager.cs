using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NijiDive.Managers.Persistence
{
    public class PersistenceManager : Manager
    {
        [SerializeField] private PersistentObject[] persistentObjects;
        [SerializeField] private string startingSceneName = "GameScene";

        public static PersistenceManager singleton;
        public static UnityEvent OnLoaded = new UnityEvent();

        private static GameObject[] persistentObjectInstances;

        // Added second condition because loading script causes null array to be set to 0 length
        public static bool IsShowingPersistentObjects => persistentObjectInstances != null && persistentObjectInstances.Length > 0;
        
        private void Awake()
        {
            if (singleton == null)
            {
                if (IsShowingPersistentObjects)
                {
                    Debug.LogError("Persistent object previews can't be shown in play mode");
                    Application.Quit();
                }

                persistentObjectInstances = SpawnPersistentObjectsAsDontDestroyOnLoad();

                DontDestroyOnLoad(gameObject);
                singleton = this;
            }
        }

        private void Start()
        {
            OnLoaded?.Invoke();

            if (singleton != this) Destroy(gameObject);
        }

        public override void Retry() { }

        public void Restart()
        {
            if (IsShowingPersistentObjects)
            {
                foreach (var gameObject in persistentObjectInstances) Destroy(gameObject);
            }

            persistentObjectInstances = null;

            Destroy(singleton.gameObject);
            singleton = null;

            SceneManager.LoadScene(startingSceneName);
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

        private GameObject[] SpawnPersistentObjectsAsDontDestroyOnLoad()
        {
            var gameObjects = SpawnPersistentObjects(Instantiate);
            foreach (var gameObject in gameObjects) DontDestroyOnLoad(gameObject);

            return gameObjects;
        }

#if UNITY_EDITOR
        private GameObject[] SpawnPersistentObjectsAsPrefabs() => SpawnPersistentObjects(PrefabUtility.InstantiatePrefab);

        /// <summary>
        /// Instantiates objects in <see cref="persistentObjects"/> as prefabs. Only meant for testing outside of play mode
        /// </summary>
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

        /// <summary>
        /// Hides prefabs instatiated in <see cref="ShowObjects"/>. Only meant for testing outside of play mode
        /// </summary>
        public void HideObjects()
        {
            if (!IsShowingPersistentObjects)
            {
                Debug.LogWarning("No persistent objects to hide");
                return;
            }

            if (!Application.isPlaying)
            {
                foreach (var gameObject in persistentObjectInstances) DestroyImmediate(gameObject);
                persistentObjectInstances = null;
            }
        }
#endif

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