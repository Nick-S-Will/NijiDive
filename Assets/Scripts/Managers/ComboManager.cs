using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Pausing;
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
        [Tooltip("Number of physics updates before confirming combo's end")]
        [SerializeField] [Min(0)] private int comboEndBufferUpdates = 2;

        private PlayerController pc;
        private Coroutine comboEndBuffer;
        private int currentCombo;

        public int CurrentCombo => currentCombo;

        public static ComboManager singleton;

        private void Awake()
        {
            if (singleton == null) singleton = this;
            else
            {
                Debug.LogError($"Multiple {typeof(ComboManager)}s exist", this);
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

        private IEnumerator ComboEndBuffer()
        {
            if (comboEndBufferUpdates == 0)
            {
                EndCombo();
                yield break;
            }

            yield return new WaitForSeconds(comboEndBufferUpdates * Time.fixedDeltaTime);
            if (pc.LastGroundCheck && !PauseManager.IsPaused) EndCombo();

            comboEndBuffer = null;
        }

        private void OnCollisionStay2D(Collision2D _)
        {
            if (currentCombo == 0) return;

            if (pc.LastGroundCheck && !PauseManager.IsPaused)
            {
                if (comboEndBuffer == null) comboEndBuffer = StartCoroutine(ComboEndBuffer());
                // TODO: Find way to end combo without needing access to map and tiles
                //var tile = MapManager.singleton.GetTile(transform.position);
                //if (tile && !(tile is BounceableTile)) EndCombo();
            }
        }

        private void OnDestroy()
        {
            if (singleton == this) singleton = null;
        }
    }
}