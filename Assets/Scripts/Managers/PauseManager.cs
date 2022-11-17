using System.Linq;
using UnityEngine;

using NijiDive.Managers.Map;

namespace NijiDive.Managers.Pausing
{
    public static class PauseManager
    {
        public static bool IsPaused { get; private set; }

        private static readonly ContactFilter2D pauseZoneFilter = GetPauseContactFilter();
        private const string pauseZoneLayerName = "Pause";

        private static ContactFilter2D GetPauseContactFilter()
        {
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask(pauseZoneLayerName));

            return filter;
        }
        
        public static void PauseAll(bool paused = true)
        {
            if (IsPaused == paused) return;

            foreach (var pauseable in Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IPauseable>())
            {
                var behaviour = (MonoBehaviour)pauseable;
                if (MapManager.singleton.PointInCenter(behaviour.transform.position))
                {
                    var collider = behaviour.GetComponent<Collider2D>();
                    if (collider && collider.IsTouching(pauseZoneFilter)) continue;
                    
                    pauseable.Pause(paused);
                }
            }
            IsPaused = paused;
        }
    }
}