using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Entities.Mobs.Enemies
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

        private Vector2 inputDirection;

        protected override void Awake()
        {
            controls.AddRange(new List<Control>() { flyingX, flyingY, stomping, shoving });

            base.Awake();
        }

        private void Update()
        {
            try { CalculateInput(); }
            catch (MissingReferenceException) { WarnTargetMissing(); }
        }

        private void FixedUpdate()
        {
            UseControls(new InputData(inputDirection));
        }

        protected override void CalculateInput()
        {
            var targetDelta = Target.position - transform.position;
            inputDirection = targetDelta.normalized;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (showPath && Target)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, Target.position);
            }
        }
    }
}