using System.Collections.Generic;
using UnityEngine;

using NijiDive.Entities;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;
using NijiDive.Health;

namespace NijiDive.Controls.Player
{
    public class PlayerController : Mob
    {
        [SerializeField] private PlayerHealthData health;
        [Header("Control Types")]
        [SerializeField] private LocalRightAnalogMoving walking;
        [SerializeField] private Jumping jumping;
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private Stomping stomping;
        [SerializeField] private Headbutting headbutting;

        [Header("Key Mapping")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode altKey = KeyCode.Escape;

        private float xInput;
        private bool jumpDown, jumpDownThisFrame, altDown, altDownThisFrame;

        public override HealthData Health => health;

        protected override void Awake()
        {
            controls = new List<Control>() { walking, jumping, weaponController, stomping, headbutting };

            base.Awake();
        }

        private void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");

            jumpDown = Input.GetKey(jumpKey);
            // Different because FixedUpdate won't always line up and catch the single frame
            if (Input.GetKeyDown(jumpKey)) jumpDownThisFrame = true;

            altDown = Input.GetKey(altKey);
            if (Input.GetKeyDown(altKey)) altDownThisFrame = true;
        }

        private void FixedUpdate()
        {
            FixedUpdate(new InputData(new Vector2(xInput, 0), jumpDown, jumpDownThisFrame, altDown, altDownThisFrame));
            jumpDownThisFrame = false;
            altDownThisFrame = false;
        }

        // Player doesn't get paused
        public override void Pause(bool pause) { }
    }
}