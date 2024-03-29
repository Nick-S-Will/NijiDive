using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.Entities.Mobs;

namespace NijiDive.Entities.Contact
{
    public class EndZone : PauseZone
    {
        public UnityEvent OnEnd;

        protected override void Start()
        {
            base.Start();

            OnEnd.AddListener(LevelManager.singleton.CompleteLevel);
        }

        protected override void Update() => base.Update();

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            if (entity.CompareTag(Constants.PLAYER_TAG)) OnEnd?.Invoke();
            else if (entity is Mob mob) mob.OnDeath?.Invoke(this, DamageType.Environment | DamageType.Void);
        }
    }
}