using UnityEngine;

namespace NijiDive.Managers
{
    public abstract class Manager : MonoBehaviour
    {
        // For resetting to level world 1
        public abstract void Retry();

        // For resetting to surface
        public abstract void Restart();

        public static void RetryAllManagers()
        {
            foreach (var manager in FindObjectsOfType<Manager>(true)) manager.Retry();
        }

        public static void RestartAllManagers()
        {
            foreach (var manager in FindObjectsOfType<Manager>(true)) manager.Restart();
        }
    }
}