using UnityEngine;

using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;
using NijiDive.Health;

namespace NijiDive.Controls.Enemies
{
    public class DirectFlyingStompEnemy : Enemy
    {
        [SerializeField] private HealthData health;
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showPath;
        [Header("Control Types")]
        [SerializeField] private LocalRightAnalogMoving flyingX;
        [SerializeField] private LocalUpAnalogMoving flyingY;
        [SerializeField] private Stomping stomping;
        [SerializeField] private Vector2 startFacingDir = Vector2.right;

        private Vector2 input;

        public override HealthData Health => health;

        protected override void Awake()
        {
            OnDeath.AddListener(Death);
            controls = new Control[] { flyingX, flyingY, stomping };
            input = startFacingDir;

            base.Awake();
        }

        private void Update()
        {
            CalculateDirection();
        }

        private void FixedUpdate()
        {
            FixedUpdate(new InputData(input));
        }

        private void CalculateDirection()
        {
            var targetDelta = target.position - transform.position;
            input = targetDelta.normalized;
        }

        private void OnDrawGizmos()
        {
            if (showPath)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}