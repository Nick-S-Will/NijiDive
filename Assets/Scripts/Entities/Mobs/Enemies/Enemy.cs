using UnityEngine;

using NijiDive.Managers.PlayerBased;
using NijiDive.Health;

namespace NijiDive.Entities.Mobs.Enemies
{
    public abstract class Enemy : Mob, ICoinDropping
    {
        [SerializeField] private HealthData health;
        [SerializeField] [Min(0)] private int coinsToDrop = 1;

        public override HealthData Health => health;
        public int CoinCount => coinsToDrop;

        protected static Transform Target => PlayerBasedManager.Player.transform;

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