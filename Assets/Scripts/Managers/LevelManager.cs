using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using NijiDive.Map;

namespace NijiDive.Managers.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "GameScene";
        [Space]
        [SerializeField] private World[] worlds;

        private static int worldIndex, levelIndex;

        public static LevelManager singleton;

        public static int WorldIndex => worldIndex;
        public static int LevelIndex => levelIndex;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(LevelManager)}s found in scene.", this);
                return;
            }
        }

        public Level GetCurrentLevel()
        {
            if (worldIndex < worlds.Length) return worlds[worldIndex].levels[levelIndex];
            else
            {
                Debug.LogError(worlds.Length == 0 ? "No levels assigned to level manager" : "Last level exceeded", this);
                return null;
            }
        }

        public void CompleteLevel()
        {
            levelIndex++;
            if (levelIndex >= worlds[worldIndex].levels.Length)
            {
                worldIndex++;
                levelIndex = 0;
            }

            // TODO: Implement upgrade scene
            SceneManager.LoadScene(gameSceneName);
        }

        public Vector3 GetCurrentWorldPlayerStart()
        {
            if (worldIndex < worlds.Length) return worlds[worldIndex].playerStartPos;
            else
            {
                Debug.LogError("World index out out bounds, couldn't get player start position", this);
                return default;
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }

        [Serializable]
        private class World
        {
            public Level[] levels;
            public Vector3 playerStartPos;
            public Color color = Color.white;
        }
    }
}