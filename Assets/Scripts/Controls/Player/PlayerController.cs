using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;
using NijiDive.Entities;
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

        private float xInput, initialGravityScale;
        private int controlCountAtAwake;
        private bool jumpDown, jumpDownThisFrame, altDown, altDownThisFrame;

        public override HealthData Health => health;

        protected override void Awake()
        {
            controls = new List<Control>() { walking, jumping, weaponController, stomping, headbutting };
            controlCountAtAwake = controls.Count;
            
            base.Awake();

            LevelManager.singleton.OnLoadLevel.AddListener(MoveToWorldStartPosition);
            LevelManager.singleton.OnLoadLevel.AddListener(Enable);
            LevelManager.singleton.OnLoadUpgrading.AddListener(Disable);
            initialGravityScale = Body2d.gravityScale;
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
            UseControls(new InputData(new Vector2(xInput, 0), jumpDown, jumpDownThisFrame, altDown, altDownThisFrame));
            jumpDownThisFrame = false;
            altDownThisFrame = false;
        }

        private void MoveToWorldStartPosition()
        {
            transform.position = LevelManager.singleton.GetCurrentWorldPlayerStart();
        }

        private void SetEnabled(bool enabled)
        {
            for (int i = 0; i < controlCountAtAwake; i++) controls[i].enabled = enabled;

            Body2d.velocity = Vector2.zero;
            Body2d.gravityScale = enabled ? initialGravityScale : 0f;
        }
        private void Enable() => SetEnabled(true);
        private void Disable() => SetEnabled(false);

        // TODO: Add pause menu and only make player pause when it's open
        public override void Pause(bool pause) { }
    }
}