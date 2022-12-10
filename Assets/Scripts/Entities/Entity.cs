using System.Collections;
using UnityEngine;

namespace NijiDive.Entities
{
    public abstract class Entity : MonoBehaviour, IPauseable
    {
        [SerializeField] private Material flashMaterial;
        [SerializeField] [Min(0f)] private float flashDuration = 0.1f;

        private Coroutine flashRoutine, periodicFlashRoutine;

        private IEnumerator FlashRoutine(Renderer renderer, float duration)
        {
            var startingMaterial = renderer.material;
            renderer.material = flashMaterial;

            yield return new WaitForSeconds(duration);
            yield return new WaitWhile(() => IsPaused);

            if (renderer) renderer.material = startingMaterial;

            flashRoutine = null;
        }
        protected void Flash(Renderer renderer)
        {
            if (flashRoutine == null) flashRoutine = StartCoroutine(FlashRoutine(renderer, flashDuration));
        }

        private IEnumerator PeriodicFlashingRoutine(Renderer renderer, float flashInterval)
        {
            while (renderer)
            {
                Flash(renderer);

                yield return new WaitForSeconds(flashInterval);
                yield return new WaitWhile(() => IsPaused);
            }

            periodicFlashRoutine = null;
        }
        protected void PeriodicFlashing(Renderer renderer, float flashInterval)
        {
            if (periodicFlashRoutine == null) periodicFlashRoutine = StartCoroutine(PeriodicFlashingRoutine(renderer, flashInterval));
        }
        protected void CancelPeriodicFlashing()
        {
            if (periodicFlashRoutine != null)
            {
                StopCoroutine(periodicFlashRoutine);
                periodicFlashRoutine = null;
            }
        }

        public virtual bool IsPaused { get; protected set; }

        public virtual void Pause(bool paused) { }
    }
}