using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Entities
{
    public abstract class Entity : MonoBehaviour, IPauseable
    {
        [SerializeField] private Material flashMaterial;
        [SerializeField] [Min(0f)] private float flashDuration = 0.1f;

        private Coroutine flashRoutine, periodicFlashRoutine, fadeOutRoutine;

        private IEnumerator FlashRoutine(Renderer renderer, float duration)
        {
            var startingMaterial = renderer.material;
            renderer.material = flashMaterial;

            yield return new PauseManager.WaitWhilePausedAndForSeconds(duration, this);

            if (renderer) renderer.material = startingMaterial;

            flashRoutine = null;
        }
        public Coroutine Flash(Renderer renderer)
        {
            if (flashRoutine == null) flashRoutine = StartCoroutine(FlashRoutine(renderer, flashDuration));

            return flashRoutine;
        }

        private IEnumerator PeriodicFlashingRoutine(Renderer renderer, float flashInterval)
        {
            while (renderer)
            {
                _ = Flash(renderer);

                yield return new PauseManager.WaitWhilePausedAndForSeconds(flashInterval, this);
            }

            periodicFlashRoutine = null;
        }
        protected void PeriodicFlashing(Renderer renderer, float flashInterval)
        {
            if (periodicFlashRoutine != null) StopCoroutine(periodicFlashRoutine);
            
            periodicFlashRoutine = StartCoroutine(PeriodicFlashingRoutine(renderer, flashInterval));
        }
        protected void CancelPeriodicFlashing()
        {
            if (periodicFlashRoutine != null)
            {
                StopCoroutine(periodicFlashRoutine);
                periodicFlashRoutine = null;
            }
        }

        private IEnumerator FadeOutRoutine(Renderer renderer, Light2D light2D, float fadeDuration)
        {
            var spriteRenderer = renderer as SpriteRenderer;
            var fadeTime = 0f;
            var startIntensity = light2D ? light2D.intensity : 0f;

            while (fadeTime < fadeDuration)
            {
                var fadePercent = fadeTime / fadeDuration;

                var color = spriteRenderer ? spriteRenderer.color : renderer.material.color;
                color.a = Mathf.Lerp(1f, 0f, fadePercent);
                if (spriteRenderer) spriteRenderer.color = color;
                else renderer.material.color = color;

                if (light2D) light2D.intensity = Mathf.Lerp(startIntensity, 0f, fadePercent);

                fadeTime += Time.deltaTime;
                yield return new PauseManager.WaitWhilePaused(this);
            }

            fadeOutRoutine = null;
        }
        public Coroutine FadeOut(float fadeDuration)
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.Log($"{name} has no renderer to fade out");
                return StartCoroutine(new PauseManager.WaitWhilePausedAndForSeconds(fadeDuration, this));
            }

            var light2D = GetComponentInChildren<Light2D>();

            return fadeOutRoutine ?? StartCoroutine(FadeOutRoutine(renderer, light2D, fadeDuration));
        }

        public virtual bool IsPaused { get; protected set; }

        public virtual void Pause(bool paused) { }
    }
}