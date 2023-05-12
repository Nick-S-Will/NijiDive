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
            set => UpdatePlayer(value);
        }

        private static PlayerController player;

        private static void UpdatePlayer(PlayerController newPlayer)
        {
            player = newPlayer;
            OnNewPlayer.Invoke();
        }
    }
}