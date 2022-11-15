using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Entities;

namespace NijiDive.Managers.Mobs
{
    public class MobManager : MonoBehaviour
    {
        public UnityEvent OnMobDeath;
        [SerializeField] private Transform target;
        [SerializeField] private float enableDistance = GameConstants.CHUNK_SIZE, destroyDistance = GameConstants.CHUNK_SIZE;

        private List<Transform> disabledMobs = new List<Transform>(), enabledMobs = new List<Transform>();

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError($"No {nameof(target)} assigned");
                enabled = false;
            }

            foreach (Transform t in transform)
            {
                if (t.GetComponent<Mob>() != null)
                {
                    disabledMobs.Add(t);
                    t.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (target == null || (disabledMobs.Count == 0 && enabledMobs.Count == 0)) enabled = false;

            EnableMobs();
            DestroyMobs();
        }

        /// <summary>
        /// Enables mobs within <see cref="enableDistance"/> of <see cref="target"/>
        /// </summary>
        private void EnableMobs()
        {
            foreach (var mob in disabledMobs.ToArray())
            {
                if (target == null) return;

                if (target.position.y - mob.position.y < enableDistance)
                {
                    mob.gameObject.SetActive(true);
                    disabledMobs.Remove(mob);
                    enabledMobs.Add(mob);
                }
            }
        }

        /// <summary>
        /// Destroys enabled mobs that exceed <see cref="destroyDistance"/> of <see cref="target"/>
        /// </summary>
        private void DestroyMobs()
        {
            foreach (var mob in enabledMobs.ToArray())
            {
                if (mob == null)
                {
                    OnMobDeath?.Invoke();
                    enabledMobs.Remove(mob);
                    continue;
                }

                if (mob.position.y - target.position.y > destroyDistance)
                {
                    enabledMobs.Remove(mob);
                    Destroy(mob.gameObject);
                }
            }
        }

        private void OnValidate()
        {
            if (target) enabled = true;
        }
    }
}