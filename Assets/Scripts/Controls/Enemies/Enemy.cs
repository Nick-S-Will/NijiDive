using UnityEngine;

using NijiDive.Health;

namespace NijiDive.Controls.Enemies
{
    public abstract class Enemy : Mob
    {
        [SerializeField] private HealthData health;

        public override HealthData Health => health;

        protected static Transform target;

        protected override void Awake()
        {
            if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Awake();
        }

        protected abstract void CalculateInput();

        protected void WarnTargetMissing()
        {
            Debug.LogWarning("Target missing", this);
            Rb2d.velocity = Vector2.zero;
            Rb2d.angularVelocity = 0;
            enabled = false;
        }

        private void OnApplicationQuit()
        {
            target = null;
        }
    }
}