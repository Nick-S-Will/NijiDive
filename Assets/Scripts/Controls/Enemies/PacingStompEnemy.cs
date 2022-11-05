using UnityEngine;

using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;
using NijiDive.Health;

namespace NijiDive.Controls.Enemies
{
    public class PacingStompEnemy : Enemy
    {
        [SerializeField] private HealthData health;
        [Header("Control Types")] 
        [SerializeField] private LocalRightDigitalMoving walking;
        [SerializeField] private Stomping stomping;
        [SerializeField] private bool startFacingRight = true;

        private float xInput;

        public override HealthData Health => health;

        protected override void Awake()
        {
            OnDeath.AddListener(Death);
            controls = new Control[] { walking, stomping };
            xInput = startFacingRight ? 1f : -1f;

            base.Awake();
        }

        private void Update()
        {
            CalculateMovement();
        }

        private void FixedUpdate()
        {
            FixedUpdate(new InputData(new Vector2(xInput, 0)));
        }

        private void CalculateMovement()
        {
            if (lastWallCheck || !lastEdgeCheck)
            {
                xInput *= -1f;
            }
        }
    }
}