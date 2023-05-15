using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using NijiDive.Map;

namespace NijiDive.Managers.Levels
{
    public class LevelManager : Manager
    {
        [SerializeField] private World[] worlds;
        [Space]

        public static LevelManager singleton;
        public static UnityEvent OnLoadLevel = new UnityEvent(), OnLoadUpgrading = new UnityEvent();

        public static int WorldIndex { get; private set; }
        public static int LevelIndex { get; private set; }
        public static bool IsUpgrading { get; private set; }

        private void OnEnable()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(LevelManager)}s found", this);
                return;
            }

            OnLoadLevel.AddListener(() => IsUpgrading = false);
            OnLoadUpgrading.AddListener(() => IsUpgrading = true);
        }

        private void Start()
        {
            OnLoadLevel.Invoke();
        }

        public override void Retry()
        {
            WorldIndex = 0;

            StartNextLevel();
        }

        public int GetLevelCount()
        {
            int count = 0;

            foreach (var world in worlds) count += world.levels.Length;

            return count;
        }

        public Level GetCurrentLevel()
        {
            if (WorldIndex < worlds.Length) return worlds[WorldIndex].levels[LevelIndex];
            else
            {
                Debug.LogError(worlds.Length == 0 ? "No levels assigned to level manager" : "Last level exceeded", this);
                return null;
            }
        }

        public string GetCurrentLevelName()
        {
            return $"{worlds[WorldIndex].name} - {LevelIndex + 1}";
        }

        public void CompleteLevel()
        {
            SceneManager.LoadScene(Constants.UPGRADE_SCENE_NAME);
            OnLoadUpgrading?.Invoke();
        }

        public void StartNextLevel()
        {
            if (++LevelIndex >= worlds[WorldIndex].levels.Length)
            {
                WorldIndex++;
                LevelIndex = 0;
            }

            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
            OnLoadLevel.Invoke();
        }

        public Vector3 GetCurrentWorldPlayerStart()
        {
            if (WorldIndex < worlds.Length) return worlds[WorldIndex].playerStartPos;
            else
            {
                Debug.LogError("World index out out bounds, couldn't get player start position", this);
                return default;
            }
        }

        private void OnDisable()
        {
            if (singleton == this) singleton = null;
        }

        [Serializable]
        private class World
        {
            public string name;
            public Level[] levels;
            public Vector3 playerStartPos;
            public Color color = Color.white;
        }
    }
}