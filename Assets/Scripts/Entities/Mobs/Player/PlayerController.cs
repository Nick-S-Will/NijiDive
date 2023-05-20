using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using NijiDive.Managers.Levels;
using NijiDive.Health;
using NijiDive.Controls;
using NijiDive.Controls.Movement;
using NijiDive.Controls.Attacks;

namespace NijiDive.Entities.Mobs.Player
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
        private bool jumpDown, jumpDownThisFrame, altDown, altDownThisFrame;

        public override HealthData Health => health;

        protected override void Awake()
        {
            controls.AddRange(new List<Control>() { walking, jumping, weaponController, stomping, headbutting });
            controlCountAtAwake = controls.Count;

            base.Awake();

            LevelManager.OnLoadLevel.AddListener(MoveToWorldStartPosition);
            LevelManager.OnLoadUpgrading.AddListener(weaponController.ReloadCurrentWeapon);
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
            UseControls(new InputData(lStick, jumpDown, jumpDownThisFrame, altDown, altDownThisFrame));
            jumpDownThisFrame = false;
            altDownThisFrame = false;
        }

        public void Retry()
        {
            health.Reset();
            ResetControls();

            EnableBaseFeatures();
        }

        private void MoveToWorldStartPosition()
        {
            transform.position = LevelManager.singleton.GetCurrentWorldPlayerStart();
        }

        public bool HasBaseFeaturesEnabled() => PerformCollisionChecks;
        public void SetBaseFeatures(bool enabled)
        {
            if (LevelManager.IsUpgrading && enabled) return;

            for (int i = 0; i < controlCountAtAwake; i++) controls[i].SetEnabled(enabled);

            Body2d.simulated = enabled;
            PerformCollisionChecks = enabled;

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

        /// <summary>
        /// Copies all <see cref="PlayerController"/> defined values from another <see cref="PlayerController"/> on this object. For easy child class set up
        /// </summary>
        [ContextMenu("Get Values")]
        public void CopyValues()
        {
            var playerControllers = GetComponents<PlayerController>();
            PlayerController playerController = null;
            foreach (var pc in playerControllers)
            {
                if (this != pc)
                {
                    playerController = pc;
                    break;
                }
            }

            if (playerController == null)
            {
                Debug.Log($"No other {nameof(PlayerController)} found on this object");
                return;
            }

            OnLandOnGround = playerController.OnLandOnGround;
            OnDeath = playerController.OnDeath;
            spriteRenderer = playerController.spriteRenderer;
            vulnerableTypes = playerController.vulnerableTypes;
            bounceSpeed = playerController.bounceSpeed;
            collisions = playerController.collisions;

            health = playerController.health;
            walking = playerController.walking;
            jumping = playerController.jumping;
            weaponController = playerController.weaponController;
            stomping = playerController.stomping;
            headbutting = playerController.headbutting;
            jumpKeys = playerController.jumpKeys;
            altKeys = playerController.altKeys;
        }
    }
}