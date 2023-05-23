using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public class JumpBlast : Blast
    {
        [Space]
        [SerializeField] private float jumpSpeedForce = 50f;
        [SerializeField] private float jumpDuration = 0.5f;

        private Coroutine jumpBlastRoutine;

        private IEnumerator JumpRoutine()
        {
            mob.DisableControls();
            mob.Velocity = Vector2.zero;

            float elapsedTime = 0f;
            while (elapsedTime < jumpDuration)
            {
                if (mob.LastCeilingCheck != null) break;

                mob.AddForce(jumpSpeedForce * Vector2.up);

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }

            _ = mob.StartCoroutine(BlastRoutine());
            mob.EnableControls();

            jumpBlastRoutine = null;
        }

        public override void TryToSpecial()
        {
            if (jumpBlastRoutine == null) jumpBlastRoutine = mob.StartCoroutine(JumpRoutine());
        }
    }
}