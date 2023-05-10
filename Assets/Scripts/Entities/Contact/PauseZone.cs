using UnityEngine;

using NijiDive.Managers.Map;
using NijiDive.Managers.Pausing;
using NijiDive.Entities.Mobs;

namespace NijiDive.Entities.Contact
{
    public class PauseZone : Entity
    {
        [Space]
        [SerializeField] [Min(0f)] private float colorOscillationSpeed = 1f;
        [SerializeField] [Range(0f, 1f)] private float minColorMultiplier = 0.5f;

        private SpriteRenderer sr;
        private Color startColor;

        public override bool IsPaused => !enabled;

        protected virtual void Start()
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            startColor = sr.color;
        }

        // Placeholder effect
        protected virtual void Update()
        {
            var colorMultiplier = 1f - Mathf.PingPong(colorOscillationSpeed * Time.time, 1f - minColorMultiplier);
            sr.color = colorMultiplier * startColor;
        }

        public override void Pause(bool paused) => enabled = !paused;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            if (entity.CompareTag(Constants.PLAYER_TAG)) PauseManager.PauseAll();
            else if (entity is Mob mob) mob.OnDeath?.Invoke(this, DamageType.Environment | DamageType.Void);
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            bool isInCenter = MapManager.singleton.PointInCenter(entity.transform.position);
            if (!isInCenter) return;

            if (entity.CompareTag(Constants.PLAYER_TAG)) PauseManager.PauseAll(false);
            else if (PauseManager.IsPaused) entity.Pause(true);
        }
    }
}