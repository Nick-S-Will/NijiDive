using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Controls.Attacks;

namespace NijiDive.Managers.PlayerBased.Combo
{
    public class ComboManager : PlayerBasedManager
    {
        private int currentCombo, maxCombo, totalKillCount;

        public int CurrentCombo => currentCombo;
        public int MaxCombo => maxCombo;
        public int TotalKillCount => totalKillCount;

        // int is the combo count at the time of the event
        public static UnityEvent<int> OnCombo = new UnityEvent<int>(), OnEndCombo = new UnityEvent<int>();
        public static ComboManager singleton;

        private void Start()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {nameof(ComboManager)}s found", this);
                gameObject.SetActive(false);
                return;
            }

            AddListenersToPlayer();
        }

        private void AddListenersToPlayer()
        {
            foreach (var attack in Player.GetControlTypes<Attacking>()) attack.OnKill.AddListener(IncreaseCombo);
            // TODO: Make special count to combo

            Player.OnLandOnGround.AddListener(CheckCollisionForEndCombo);
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

        private void CheckCollisionForEndCombo()
        {
            if (currentCombo == 0) return;

            if (Player.LastGroundCheck && !PauseManager.IsPaused) EndCombo();
        }

        private void OnDestroy()
        {
            if (singleton != this) return;
            
            singleton = null;
        }
    }
}