using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Map;

namespace NijiDive.Managers.Pausing
{
    public static class PauseManager
    {
        public static UnityEvent OnSetPause = new UnityEvent();
        public static bool IsPaused { get; private set; }

        private static readonly ContactFilter2D pauseZoneFilter = GetPauseContactFilter();
        private const string PAUSE_ZONE_LAYER_NAME = "Pause";

        private static ContactFilter2D GetPauseContactFilter()
        {
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask(PAUSE_ZONE_LAYER_NAME));

            return filter;
        }

        public static void PauseAll(bool paused = true)
        {
            if (IsPaused == paused) return;

            foreach (var pauseable in Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IPauseable>())
            {
                var behaviour = (MonoBehaviour)pauseable;
                if (MapManager.singleton == null || MapManager.singleton.PointInCenter(behaviour.transform.position))
                {
                    var collider = behaviour.GetComponent<Collider2D>();
                    if (collider && collider.IsTouching(pauseZoneFilter)) continue;

                    pauseable.Pause(paused);
                }
            }
            IsPaused = paused;

            OnSetPause?.Invoke();
        }

        public class WaitWhilePaused : CustomYieldInstruction
        {
            private readonly IPauseable pauseable;

            public override bool keepWaiting => pauseable != null ? pauseable.IsPaused : IsPaused;

            /// <summary>
            /// Waits while <paramref name="pauseable"/> is paused
            /// </summary>
            /// <param name="pauseable">Used to query <see cref="IPauseable.IsPaused"/>. If left null, <see cref="IsPaused"/> is queried instead</param>
            public WaitWhilePaused(IPauseable pauseable = null)
            {
                this.pauseable = pauseable;
            }
        }

        public class WaitWhilePausedAndForSeconds : CustomYieldInstruction
        {
            private readonly WaitWhilePaused pauseYield;
            private float elapsedTime;
            private readonly float durationInSeconds;

            public override bool keepWaiting
            {
                get
                {
                    if (pauseYield.keepWaiting) return true;

                    elapsedTime += Time.deltaTime;
                    return elapsedTime < durationInSeconds;
                }
            }

            /// <summary>
            /// Waits while <paramref name="pauseable"/> is paused and increases <see cref="elapsedTime"/> if not until it reaches <paramref name="seconds"/>
            /// </summary>
            /// <param name="seconds">Amount of seconds the </param>
            /// <param name="pauseable">Used to query <see cref="IPauseable.IsPaused"/>. If left null, <see cref="IsPaused"/> is queried instead</param>
            public WaitWhilePausedAndForSeconds(float seconds, IPauseable pauseable = null)
            {
                pauseYield = new WaitWhilePaused(pauseable);
                elapsedTime = 0f;
                durationInSeconds = seconds;
            }
        }
    }
}