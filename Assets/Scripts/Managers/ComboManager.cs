using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
using NijiDive.Controls.Player;
using NijiDive.Controls.Attacks;

namespace NijiDive.Managers.Combo
{
    [RequireComponent(typeof(PlayerController))]
    public class ComboManager : MonoBehaviour
    {
        // int is the combo count at the time of the event
        public UnityEvent<int> OnCombo, OnEndCombo;

        private PlayerController pc;
        private int currentCombo;

        public int CurrentCombo => currentCombo;

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
            pc = GetComponent<PlayerController>();
            var stomping = pc.GetControlType<Stomping>();
            var shooting = pc.GetControlType<WeaponController>();

            stomping.OnKill.AddListener(IncreaseCombo);
            shooting.OnKill.AddListener(IncreaseCombo);
        }

        private void IncreaseCombo()
        {
            currentCombo++;
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

            if (pc.LastGroundCheck && !PauseManager.IsPaused) EndCombo();
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}