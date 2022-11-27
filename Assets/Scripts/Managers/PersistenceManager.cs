using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NijiDive.Managers.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        public static UnityEvent OnLoaded = new UnityEvent();
        [Space]
        [SerializeField] private GameObject[] persitentObjectPrefabs;

        private static Scene persistentScene;
        private static bool loaded;

        private void Awake()
        {
            if (!loaded)
            {
                GameObject obj = null;
                foreach (var prefab in persitentObjectPrefabs)
                {
                    obj = Instantiate(prefab);
                    DontDestroyOnLoad(obj);
                }
                if (obj != null) persistentScene = obj.scene;

                loaded = true;
            }

            OnLoaded?.Invoke();
            Destroy(gameObject);
        }

        public static T FindPersistentObjectOfType<T>()
        {
            foreach (var gameObject in persistentScene.GetRootGameObjects())
            {
                T obj = gameObject.GetComponent<T>();
                if (obj != null) return obj;
            }

            return default;
        }
    }
}