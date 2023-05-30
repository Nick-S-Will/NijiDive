using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Entities.Mobs;
using NijiDive.Utilities;

namespace NijiDive.Entities.Terrain {
    public class DamageSurface : MonoBehaviour
    {
        [SerializeField] [Min(0f)] private float contactDurationForDamage = 1f;
        [SerializeField] private DamageType damageType = DamageType.Environment;
        [SerializeField] [Min(1)] private int damage = 1;
        [Space]
        [SerializeField] private BoxCollider2D physicalCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] [Min(1)] private int boxWidth = 1, boxHeight = 1;

        /// <summary>
        /// Mob is the mob in contact, int is the number of damager surfaces it's contacting
        /// </summary>
        private static Dictionary<Mob, int> contactMobs = new Dictionary<Mob, int>();

        private IEnumerator ContactStopwatchRoutine(Mob contactMob)
        {
            if (contactMobs.ContainsKey(contactMob))
            {
                contactMobs[contactMob] += 1;
                yield break;
            }
            else contactMobs.Add(contactMob, 1);

            var contactTime = Time.time;
            while (contactMobs.ContainsKey(contactMob))
            {
                var percentage = Mathf.Min((Time.time - contactTime) / contactDurationForDamage, 1f);
                // TODO: Add visual effect of poison color filling up player

                if (percentage == 1f)
                {
                    contactMob.TryDamage(this, damage, damageType, contactMob.transform.position);
                    contactTime = Time.time;
                }

                yield return new PauseManager.WaitWhilePausedAndForSeconds(Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var mob = collision.collider.GetComponent<Mob>();
            if (mob == null) return;

            StartCoroutine(ContactStopwatchRoutine(mob));
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var mob = collision.collider.GetComponent<Mob>();
            if (mob == null || !contactMobs.ContainsKey(mob)) return;

            if (contactMobs[mob] == 1) contactMobs.Remove(mob);
            else contactMobs[mob] -= 1;
        }

        private void OnValidate()
        {
            var boxSize = new Vector2Int(boxWidth, boxHeight);
            if (physicalCollider) physicalCollider.size = boxSize;
            if (spriteRenderer) ScriptingUtilities.DelayCall(() => { if (spriteRenderer) spriteRenderer.size = boxSize; });
        }
    }
}