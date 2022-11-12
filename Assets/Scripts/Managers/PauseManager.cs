using System.Linq;
using UnityEngine;

using NijiDive.Managers.Map;

namespace NijiDive.Managers.Pausing
{
    public static class PauseManager
    {
        public static bool IsPaused { get; private set; }

        public static void SetPauseAll(bool paused)
        {
            if (IsPaused == paused) return;

            foreach (var pauseable in Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IPauseable>())
            {
                if (MapManager.singleton.PointInCenter(((MonoBehaviour)pauseable).transform.position))
                {
                    pauseable.SetPaused(paused);
                }
            }
            IsPaused = paused;
        }
    }
}