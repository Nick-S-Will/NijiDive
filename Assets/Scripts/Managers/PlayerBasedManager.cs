using UnityEngine;
using UnityEngine.Events;

using NijiDive.Entities.Mobs.Player;

namespace NijiDive.Managers.PlayerBased
{
    public abstract class PlayerBasedManager : Manager
    {
        public static UnityEvent OnNewPlayer = new UnityEvent();

        public static PlayerController Player
        {
            get
            {
                if (player == null)
                {
                    player = FindObjectOfType<PlayerController>();
                    if (player == null) Debug.LogError($"No {nameof(PlayerController)} found");
                }

                return player;
            }
            set
            {
                player = value;
                OnNewPlayer.Invoke();
            }
        }

        private static PlayerController player;

        [ContextMenu("Invoke On New Player")]
        public void InvokeOnNewPlayer() => OnNewPlayer.Invoke();
    }
}