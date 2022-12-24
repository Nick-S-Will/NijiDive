using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Entities.Mobs.Enemies
{
    public class PacingStompEnemy : Enemy
    {
        [Header("Control Types")]
        [SerializeField] private LocalRightDigitalMoving walking;
        [SerializeField] private Stomping stomping;
        [SerializeField] private Shoving shoving;
        [SerializeField] private bool startFacingRight = true;

        private float xInput;

        protected override void Awake()
        {
            controls = new List<Control>() { walking, stomping, shoving };
            xInput = startFacingRight ? 1f : -1f;

            base.Awake();
        }

        private void Update()
        {
            CalculateInput();
        }

        private void FixedUpdate()
        {
            UseControls(new InputData(new Vector2(xInput, 0)));
        }

        protected override void CalculateInput()
        {
            if (LastWallCheck || (!LastEdgeCheck && LastGroundCheck))
            {
                xInput *= -1f;
            }
        }
    }
}