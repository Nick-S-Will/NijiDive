using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using NijiDive.Map;

namespace NijiDive.Managers.Levels
{
    public class LevelManager : Manager
    {
        [SerializeField] private string gameSceneName = "GameScene", upgradeSceneName = "UpgradeScene";
        [Space]
        [SerializeField] private World[] worlds;
        [Space]
        public UnityEvent OnLoadLevel, OnLoadLevelPostStart, OnLoadUpgrading;

        public static LevelManager singleton;

        public int WorldIndex { get; private set; }
        public int LevelIndex { get; private set; }

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(LevelManager)}s found in scene.", this);
                return;
            }

            OnLoadLevel.AddListener(InvokeOnLoadLevelPostStart);
        }

        private void Start()
        {
            OnLoadLevel?.Invoke();
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

        private IEnumerator DelayEventOneFrameRoutine(UnityEvent unityEvent)
        {
            yield return null;

            unityEvent?.Invoke();
        }
        private void InvokeOnLoadLevelPostStart() => _ = StartCoroutine(DelayEventOneFrameRoutine(OnLoadLevelPostStart));

        public void CompleteLevel()
        {
            SceneManager.LoadScene(upgradeSceneName);
            OnLoadUpgrading?.Invoke();
        }

        public void StartNextLevel()
        {
            if (++LevelIndex >= worlds[WorldIndex].levels.Length)
            {
                WorldIndex++;
                LevelIndex = 0;
            }

            SceneManager.LoadScene(gameSceneName);
            OnLoadLevel?.Invoke();
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

        private void OnDestroy()
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