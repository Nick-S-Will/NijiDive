using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.Controls.Player;
using NijiDive.Controls.Attacks;

namespace NijiDive.Entities
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

            if (entity is PlayerController player)
            {
                OnEnd?.Invoke();
                player.GetControlType<WeaponController>().ReloadCurrentWeapon();
            }
            else if (entity is Mob mob) mob.OnDeath?.Invoke(mob, this, DamageType.Environment | DamageType.Void);
        }
    }
}