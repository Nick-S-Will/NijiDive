using UnityEngine;

using NijiDive.Managers.Entities;
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

        protected static Transform Target => EntityManager.singleton.Target;

        protected override void Awake() => base.Awake();

        protected abstract void CalculateInput();

        protected void WarnTargetMissing()
        {
            Debug.LogWarning("Target missing", this);
            Body2d.velocity = Vector2.zero;
            Body2d.angularVelocity = 0;
            enabled = false;
        }
    }
}