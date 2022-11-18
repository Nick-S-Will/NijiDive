using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Controls.Enemies
{
    public class DirectFlyingStompEnemy : Enemy
    {
        [Header("Path Visualizer")]
        [SerializeField] private Color gizmoColor = Color.white;
        [SerializeField] private bool showPath;
        [Header("Control Types")]
        [SerializeField] private LocalRightAnalogMoving flyingX;
        [SerializeField] private LocalUpAnalogMoving flyingY;
        [SerializeField] private Stomping stomping;
        [SerializeField] private Shoving shoving;

        private Vector2 input;

        protected override void Awake()
        {
            controls = new List<Control>() { flyingX, flyingY, stomping, shoving };

            base.Awake();
        }

        private void Update()
        {
            try { CalculateInput(); }
            catch (MissingReferenceException) { WarnTargetMissing(); }
        }

        private void FixedUpdate()
        {
            FixedUpdate(new InputData(input));
        }

        protected override void CalculateInput()
        {
            var targetDelta = target.position - transform.position;
            input = targetDelta.normalized;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (showPath && target)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}