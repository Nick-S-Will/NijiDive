using UnityEngine;

using NijiDive.Managers.Levels;

namespace NijiDive.Managers.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(UIManager)}s found in scene", this);
                gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            if (LevelManager.singleton.WorldIndex == 0) gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}