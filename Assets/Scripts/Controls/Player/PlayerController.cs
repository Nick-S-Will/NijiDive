using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Controls.Player
{
    public class PlayerController : Mob
    {
        [Header("Control Types")]
        [SerializeField] private Walking walking;
        [SerializeField] private Jumping jumping;
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private Stomping stomping;
        [SerializeField] private Headbutting headbutting;

        [Header("Key Mapping")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        public KeyCode JumpKey => jumpKey;

        private float xInput;
        private bool jumpDown, jumpDownThisFrame;

        protected override void Awake()
        {
            controls = new Control[] { walking, jumping, weaponController, stomping, headbutting };

            base.Awake();
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            jumpDown = Input.GetKey(JumpKey);
            // Different because FixedUpdate won't always line up and catch the single frame
            if (Input.GetKeyDown(jumpKey)) jumpDownThisFrame = true;
        }

        private void FixedUpdate()
        {
            FixedUpdate(new Inputs(new Vector2(xInput, 0), jumpDown, jumpDownThisFrame));
            jumpDownThisFrame = false;
        }
    }
}