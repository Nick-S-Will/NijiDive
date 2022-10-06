using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive.Controls
{
    public class PlayerController : HumanoidControls
    {
        [SerializeField] private bool moving = true, jumping = true, variableJumping = true, clampFalling = true;

        [Header("Key Mapping")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        private float xInput;

        protected override void Awake() => base.Awake();

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            if (jumping && Input.GetKeyDown(jumpKey)) TryJump();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (moving) Move(xInput);
            if (variableJumping) TryAddVariableGravity(Input.GetKey(jumpKey));
            if (clampFalling) ClampFallSpeed();
        }
    }
}