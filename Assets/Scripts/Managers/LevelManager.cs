using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Map;

namespace NijiDive.Managers.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Level[][] levels;

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

            DontDestroyOnLoad(gameObject);
        }

        public Level GetCurrentLevel()
        {
            if (worldIndex < levels.Length) return levels[worldIndex][levelIndex];
            else
            {
                Debug.LogError(levels.Length == 0 ? "No levels assigned to level manager" : "Last level exceeded", this);
                return null;
            }
        }

        public void CompleteLevel()
        {
            levelIndex++;
            if (levelIndex >= levels[worldIndex].Length)
            {
                worldIndex++;
                levelIndex = 0;
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}