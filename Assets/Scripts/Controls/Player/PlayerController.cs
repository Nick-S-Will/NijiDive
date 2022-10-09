using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive.Controls.Player
{
    public class PlayerController : HumanoidControls
    {
        [Header("Key Mapping")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        public KeyCode JumpKey => jumpKey;

        private float xInput;

        protected override void Awake() => base.Awake();

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(jumpKey)) TryJump();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Move(xInput);
            TryAddVariableGravity(Input.GetKey(jumpKey));
            TryClampVerticalSpeed();
        }
    }
}