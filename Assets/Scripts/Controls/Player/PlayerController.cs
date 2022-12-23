using System.Linq;
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
        [SerializeField] private KeyCode[] jumpKeys = new KeyCode[] { KeyCode.Space };
        [SerializeField] private KeyCode[] altKeys = new KeyCode[] { KeyCode.Escape };

        private Vector2 lStick;
        private int controlCountAtAwake;
        private bool jumpDown, jumpDownThisFrame, altDown, altDownThisFrame, performCollisionChecks;

        public override HealthData Health => health;

        protected override void Awake()
        {
            controls = new List<Control>() { walking, jumping, weaponController, stomping, headbutting };
            controlCountAtAwake = controls.Count;
            performCollisionChecks = true;

            base.Awake();

            LevelManager.singleton.OnLoadLevel.AddListener(MoveToWorldStartPosition);
        }

        private void Update()
        {
            lStick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            jumpDown = jumpKeys.Any(key => Input.GetKey(key));
            // Different because FixedUpdate won't always line up and catch the single frame
            if (jumpKeys.Any(key => Input.GetKeyDown(key))) jumpDownThisFrame = true;

            altDown = altKeys.Any(key => Input.GetKey(key));
            if (altKeys.Any(key => Input.GetKeyDown(key))) altDownThisFrame = true;
        }

        private void FixedUpdate()
        {
            UseControls(new InputData(lStick, jumpDown, jumpDownThisFrame, altDown, altDownThisFrame), performCollisionChecks);
            jumpDownThisFrame = false;
            altDownThisFrame = false;
        }

        public void Retry()
        {
            health.Reset();
            weaponController.Reset();

            EnableBaseFeatures();
        }

        private void MoveToWorldStartPosition()
        {
            transform.position = LevelManager.singleton.GetCurrentWorldPlayerStart();
        }

        public bool HasBaseFeaturesEnabled() => performCollisionChecks;
        public void SetBaseFeatures(bool enabled)
        {
            for (int i = 0; i < controlCountAtAwake; i++) controls[i].SetEnabled(enabled);

            Body2d.simulated = enabled;
            performCollisionChecks = enabled;

            var animator = GetComponentInChildren<Animator>();
            if (animator) animator.enabled = enabled;
        }
        public void EnableBaseFeatures() => SetBaseFeatures(true);
        public void DisableBaseFeatures() => SetBaseFeatures(false);

        public override void Pause(bool pause) { }

        protected override void Death(MonoBehaviour sourceBehaviour, DamageType damageType)
        {
            DisableBaseFeatures();
        }
    }
}