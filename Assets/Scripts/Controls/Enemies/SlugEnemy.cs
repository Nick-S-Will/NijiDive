using UnityEngine;

using NijiDive.Controls.Movement;

namespace NijiDive.Controls.Enemies
{
    public class SlugEnemy : Mob
    {
        [Header("Control Types")]
        [SerializeField] private Walking walking;

        private float xInput;

        protected override void Awake()
        {
            controls = new Control[] { walking };

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
            xInput = 0f;
        }
    }
}