using UnityEngine;

using NijiDive.Managers.PlayerBased;
using NijiDive.Entities.Mobs.Player;

namespace NijiDive.MenuItems.Upgrades
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Character")]
    public class Character : Upgrade
    {
        [Space]
        [SerializeField] private PlayerController newPlayerControllerPrefab;

        public override void Equip()
        {
            base.Equip();

            var oldPlayer = PlayerBasedManager.Player.transform;
            Destroy(oldPlayer.gameObject);
            
            var newPlayer = Instantiate(newPlayerControllerPrefab, oldPlayer.position, oldPlayer.rotation, oldPlayer.parent);
            DontDestroyOnLoad(newPlayer);
            PlayerBasedManager.Player = newPlayer;
        }
    }
}