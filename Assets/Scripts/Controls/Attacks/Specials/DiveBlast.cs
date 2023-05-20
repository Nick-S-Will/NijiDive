using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public class DiveBlast : Blast
    {
        [Space]
        [SerializeField] private float hangDuration = 0.2f;
        [SerializeField] private float diveSpeedForce = 50f, diveDuration = 0.5f;

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
    }
}