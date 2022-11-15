using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Map;
using NijiDive.Map.Tiles;
using NijiDive.Controls.Player;
using NijiDive.Controls.Attacks;

namespace NijiDive.Managers.Combo
{
    [RequireComponent(typeof(PlayerController))]
    public class ComboManager : MonoBehaviour
    {
        public UnityEvent<int> OnCombo, OnEndCombo;

        private PlayerController pc;
        private int currentCombo;

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

        private void OnCollisionStay2D(Collision2D _)
        {
            if (currentCombo == 0) return;

            if (pc.LastGroundCheck)
            {
                var tile = MapManager.singleton.GetTile(transform.position);
                if (tile && !(tile is BounceableTile)) EndCombo();
            }
        }
    }
}