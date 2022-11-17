using UnityEngine;

using NijiDive.Managers.Map;
using NijiDive.Managers.Pausing;
using NijiDive.Controls.Player;

namespace NijiDive.Entities
{
    public class PauseZone : Entity
    {
        [SerializeField] [Min(0f)] private float colorOscillationSpeed = 1f;
        [SerializeField] [Range(0f, 1f)] private float minColorMultiplier = 0.5f;

        private SpriteRenderer sr;
        private Color startColor;
        private float timeElapsed;

        public override bool IsPaused => !enabled;

        private void Start()
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            startColor = sr.color;
        }

        // Placeholder effect
        private void Update()
        {
            var colorMultiplier = 1f - Mathf.PingPong(colorOscillationSpeed * timeElapsed, 1f - minColorMultiplier);
            sr.color = colorMultiplier * startColor;
            timeElapsed += Time.deltaTime;
        }

        public override void Pause(bool paused) => enabled = !paused;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            if (entity is PlayerController) PauseManager.PauseAll();
            else if (entity is Mob mob) mob.OnDeath?.Invoke(mob, gameObject, DamageType.Environment | DamageType.Void);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            bool isInCenter = MapManager.singleton.PointInCenter(entity.transform.position);
            if (!isInCenter) return;

            if (entity is PlayerController) PauseManager.PauseAll(false);
            else if (PauseManager.IsPaused) entity.Pause(true);
        }
    }
}