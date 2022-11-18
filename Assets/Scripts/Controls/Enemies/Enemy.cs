using UnityEngine;

using NijiDive.Managers.Mobs;
using NijiDive.Entities;
using NijiDive.Health;

namespace NijiDive.Controls.Enemies
{
    public abstract class Enemy : Mob, ICoinDropping
    {
        [SerializeField] private HealthData health;
        [SerializeField] [Min(0)] private int coinsToDrop = 1;

        public override HealthData Health => health;
        public int CoinCount => coinsToDrop;

        protected static Transform target;

        protected override void Awake() => base.Awake();

        private void Start()
        {
            if (target == null) target = MobManager.singleton.Target;
        }

        protected abstract void CalculateInput();

        protected void WarnTargetMissing()
        {
            Debug.LogWarning("Target missing", this);
            Body2d.velocity = Vector2.zero;
            Body2d.angularVelocity = 0;
            enabled = false;
        }

        private void OnApplicationQuit()
        {
            target = null;
        }
    }
}