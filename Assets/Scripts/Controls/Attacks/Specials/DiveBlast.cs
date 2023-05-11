using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public class DiveBlast : Special
    {
        [SerializeField] private float hangDuration = 0.2f, diveSpeedForce = 50f, diveDuration = 0.5f;
        [SerializeField] private Vector2 blastSize = Vector2.one;
        [SerializeField] private Sprite blastSprite;
        [SerializeField] private float blastDuration = 0.2f;

        private Coroutine diveBlastRoutine;

        protected override void Use()
        {
            if (diveBlastRoutine == null) diveBlastRoutine = mob.StartCoroutine(DiveRoutine());
        }

        private IEnumerator DiveRoutine()
        {
            mob.DisableControls();

            float elapsedTime = 0f;
            while (elapsedTime < hangDuration)
            {
                mob.Velocity = Vector2.zero;

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }

            elapsedTime = 0;
            while (elapsedTime < diveDuration)
            {
                if (mob.LastGroundCheck != null) break;

                mob.AddForce(diveSpeedForce * Vector2.down);

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }

            _ = mob.StartCoroutine(BlastRoutine());
            mob.EnableControls();

            diveBlastRoutine = null;
        }

        private IEnumerator BlastRoutine()
        {
            var blastObject = new GameObject("Blast");
            blastObject.transform.position = mob.transform.position;
            var blastRenderer = blastObject.AddComponent<SpriteRenderer>();
            blastRenderer.sprite = blastSprite;
            blastRenderer.drawMode = SpriteDrawMode.Sliced;
            blastRenderer.size = blastSize;
            blastRenderer.sortingOrder = -4;
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