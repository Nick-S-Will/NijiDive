using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Controls.Attacks;

namespace NijiDive.Managers.PlayerBased.Combo
{
    public class ComboManager : PlayerBasedManager
    {
        // int is the combo count at the time of the event
        public UnityEvent<int> OnCombo, OnEndCombo;

        private int currentCombo, maxCombo, totalKillCount;

        public int CurrentCombo => currentCombo;
        public int MaxCombo => maxCombo;
        public int TotalKillCount => totalKillCount;

        public static ComboManager singleton;

        private void OnEnable()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(ComboManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }

            OnNewPlayer.AddListener(GetPlayerAndAddListeners);
        }

        private void GetPlayerAndAddListeners()
        {
            var stomping = Player.GetControlType<Stomping>();
            var shooting = Player.GetControlType<WeaponController>();

            stomping.OnKill.AddListener(IncreaseCombo);
            shooting.OnKill.AddListener(IncreaseCombo);
            Player.OnDeath.AddListener((monoBehaviour, damageType) => EndCombo());
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

            if (Player.LastGroundCheck && !PauseManager.IsPaused) EndCombo();
        }

        private void OnDisable()
        {
            if (singleton == this) singleton = null;
        }
    }
}