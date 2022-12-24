using UnityEngine;

namespace NijiDive.Managers
{
    public abstract class Manager : MonoBehaviour
    {
        // For resetting to world 1
        public abstract void Retry();

        public static void RetryAllManagers()
        {
            foreach (var manager in FindObjectsOfType<Manager>(true)) manager.Retry();
        }
    }
}