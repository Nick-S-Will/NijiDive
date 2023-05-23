using System;
using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.Controls.Attacks.Specials
{
    [Serializable]
    public class ZoneBlast : Blast
    {
        [Space]
        [SerializeField] private float hangDuration = 0.2f;

        private Coroutine zoneBlastRoutine;

        private IEnumerator ZoneRoutine()
        {
            mob.DisableControls();

            _ = mob.StartCoroutine(BlastRoutine());

            float elapsedTime = 0f;
            while (elapsedTime < hangDuration)
            {
                mob.Velocity = Vector2.zero;

                yield return new WaitForSeconds(Time.fixedDeltaTime);
                elapsedTime += Time.fixedDeltaTime;
            }

            mob.EnableControls();

            zoneBlastRoutine = null;
        }

        public override void TryToSpecial()
        {
            if (zoneBlastRoutine == null) zoneBlastRoutine = mob.StartCoroutine(ZoneRoutine());
        }
    }
}