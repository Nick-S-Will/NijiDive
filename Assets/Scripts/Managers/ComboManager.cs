using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Entities.Mobs.Player;
using NijiDive.Controls.Attacks;

namespace NijiDive.Managers.Combo
{
    [RequireComponent(typeof(PlayerController))]
    public class ComboManager : Manager
    {
        // int is the combo count at the time of the event
        public UnityEvent<int> OnCombo, OnEndCombo;

        private PlayerController playerController;
        private int currentCombo, maxCombo, totalKillCount;

        public int CurrentCombo => currentCombo;
        public int MaxCombo => maxCombo;
        public int TotalKillCount => totalKillCount;

        public static ComboManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(ComboManager)}s found in scene", this);
                gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            var stomping = playerController.GetControlType<Stomping>();
            var shooting = playerController.GetControlType<WeaponController>();

            playerController.OnDeath.AddListener((monoBehaviour, damageType) => EndCombo());
            stomping.OnKill.AddListener(IncreaseCombo);
            shooting.OnKill.AddListener(IncreaseCombo);
        }

        public override void Retry()
        {
            maxCombo = 0;
            totalKillCount = 0;
        }

        private void IncreaseCombo()
        {
            currentCombo++;
            if (currentCombo > maxCombo) maxCombo = currentCombo;
            totalKillCount++;
            OnCombo?.Invoke(currentCombo);
        }

        private void EndCombo()
        {
            OnEndCombo?.Invoke(currentCombo);

            currentCombo = 0;
        }

        private void OnCollisionEnter2D(Collision2D _)
        {
            if (currentCombo == 0) return;

            if (playerController.LastGroundCheck && !PauseManager.IsPaused) EndCombo();
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}