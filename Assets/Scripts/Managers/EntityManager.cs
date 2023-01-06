using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers.Persistence;
using NijiDive.Entities;
using NijiDive.Entities.Mobs;

namespace NijiDive.Managers.Entities
{
    public class EntityManager : Manager
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private float enableDistance = Constants.CHUNK_SIZE, destroyDistance = Constants.CHUNK_SIZE;

        private List<Transform> disabledEntities, enabledEntities;
        private Transform target;

        public Entity[] EnabledEntities => enabledEntities.Select(t => t.GetComponent<Entity>()).ToArray();
        public Entity[] DisabledEntities => disabledEntities.Select(t => t.GetComponent<Entity>()).ToArray();
        public Entity[] Entities => EnabledEntities.Concat(DisabledEntities).ToArray();
        public Mob[] EnabledMobs => GetMobs(enabledEntities);
        public Mob[] DisabledMobs => GetMobs(disabledEntities);
        public Mob[] Mobs => EnabledMobs.Concat(DisabledMobs).ToArray();
        public Transform Target => target;

        public static EntityManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(EntityManager)}s found", this);
                gameObject.SetActive(false);
                enabled = false;
                return;
            }

            PersistenceManager.OnLoaded.AddListener(SetTarget);
        }

        private void Start()
        {
            disabledEntities = new List<Transform>();
            enabledEntities = new List<Transform>();
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Entity>() != null)
                {
                    disabledEntities.Add(child);
                    child.gameObject.SetActive(false);
                }
            }
        }

        public override void Retry() { }

        private void Update()
        {
            if (disabledEntities.Count == 0 && enabledEntities.Count == 0) enabled = false;
            if (target == null) return;

            EnableEntities();
            DestroyEntities();
        }

        private void SetTarget()
        {
            target = GameObject.FindWithTag(targetTag).transform;
        }

        /// <summary>
        /// Enables mobs within <see cref="enableDistance"/> of <see cref="target"/>
        /// </summary>
        private void EnableEntities()
        {
            foreach (var mob in disabledEntities.ToArray())
            {
                if (target == null) return;

                if (target.position.y - mob.position.y < enableDistance)
                {
                    mob.gameObject.SetActive(true);
                    disabledEntities.Remove(mob);
                    enabledEntities.Add(mob);
                }
            }
        }

        /// <summary>
        /// Destroys enabled mobs that exceed <see cref="destroyDistance"/> of <see cref="target"/>
        /// </summary>
        private void DestroyEntities()
        {
            foreach (var mob in enabledEntities.ToArray())
            {
                if (target == null) return;

                if (mob == null)
                {
                    _ = enabledEntities.Remove(mob);
                    continue;
                }

                if (mob.position.y - target.position.y > destroyDistance)
                {
                    _ = enabledEntities.Remove(mob);
                    Destroy(mob.gameObject);
                }
            }
        }

        private Mob[] GetMobs(List<Transform> transforms)
        {
            var mobs = new List<Mob>();
            foreach (var t in transforms)
            {
                var mob = t.GetComponent<Mob>();
                if (mob) mobs.Add(mob);
            }

            return mobs.ToArray();
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}