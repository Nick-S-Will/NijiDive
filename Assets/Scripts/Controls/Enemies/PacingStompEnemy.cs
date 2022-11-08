using UnityEngine;

using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Controls.Enemies
{
    public class PacingStompEnemy : Enemy
    {
        [Header("Control Types")]
        [SerializeField] private LocalRightDigitalMoving walking;
        [SerializeField] private Stomping stomping;
        [SerializeField] private bool startFacingRight = true;

        private float xInput;

        protected override void Awake()
        {
            controls = new Control[] { walking, stomping };
            xInput = startFacingRight ? 1f : -1f;

            base.Awake();
        }

        private void Update()
        {
            CalculateInput();
        }

        private void FixedUpdate()
        {
            FixedUpdate(new InputData(new Vector2(xInput, 0)));
        }

        protected override void CalculateInput()
        {
            if (lastWallCheck || (!lastEdgeCheck && lastGroundCheck))
            {
                xInput *= -1f;
            }
        }
    }
}