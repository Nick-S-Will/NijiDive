using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;
using NijiDive.Entities.Contact;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public abstract class Blast : Special
    {
        [Space]
        [SerializeField] private GameObject blastPrefab;
        [SerializeField] private Vector2 blastPosition = Vector2.zero;
        [Tooltip("Set to 0 for blast to exist indefinitely")]
        [SerializeField] [Min(0f)] private float blastDuration = 0.2f;

        protected Coroutine blastRoutine;

        private IEnumerator DamageSpriteBounds(SpriteRenderer spriteRenderer)
        {
            var blastBounds = new Bounds(spriteRenderer.transform.position, spriteRenderer.size);
            float elapsedTime = 0f;
            while (elapsedTime < blastDuration)
            {
                var samples = SampleBounds(blastBounds);
                TryDamageSamples(mob, samples, damageType, damage);

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }
        }

        protected IEnumerator BlastRoutine()
        {
            GameObject blastObject = UnityEngine.Object.Instantiate(blastPrefab, mob.transform.position + (Vector3)blastPosition, Quaternion.identity);

            if (blastObject.GetComponentInChildren<PauseZone>()) yield break;
            else
            {
                var spriteRenderer = blastObject.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer == null) Debug.LogError($"{nameof(blastPrefab)} must have a sprite in it's lineage to {nameof(DamageSpriteBounds)}");
                else yield return DamageSpriteBounds(spriteRenderer);
            }

            if (blastDuration > 0f) UnityEngine.Object.Destroy(blastObject);
        }
    }
}