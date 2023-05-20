using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public abstract class Blast : Special
    {
        [Space]
        [SerializeField] private Vector2 blastPosition = Vector2.zero;
        [SerializeField] private Vector2 blastSize = Vector2.one;
        [SerializeField] private Sprite blastSprite;
        [SerializeField] private float blastDuration = 0.2f;

        protected Coroutine blastRoutine;

        protected IEnumerator BlastRoutine()
        {
            var blastObject = new GameObject("Blast");
            blastObject.transform.position = mob.transform.position + (Vector3)blastPosition;
            var blastRenderer = blastObject.AddComponent<SpriteRenderer>();
            blastRenderer.sprite = blastSprite;
            blastRenderer.drawMode = SpriteDrawMode.Sliced;
            blastRenderer.size = blastSize;
            blastRenderer.sortingOrder = -5;
            var blastBounds = new Bounds(blastObject.transform.position, blastSize);

            float elapsedTime = 0f;
            while (elapsedTime < blastDuration)
            {
                var samples = SampleBounds(blastBounds);
                TryDamageSamples(mob, samples, damageType, damage);

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }

            UnityEngine.Object.Destroy(blastObject);
        }
    }
}