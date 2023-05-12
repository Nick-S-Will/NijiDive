using UnityEngine;

using NijiDive.Managers.PlayerBased;
using NijiDive.Entities.Mobs.Player;

namespace NijiDive.MenuItems
{
    [CreateAssetMenu(menuName = "NijiDive/Menu Items/Character")]
    public class Character : Upgrade
    {
        [SerializeField] private PlayerController newPlayerControllerPrefab;

        public override void Equip()
        {
            base.Equip();

            var oldPlayer = GameObject.FindWithTag(Constants.PLAYER_TAG).transform;
            var newPlayer = Instantiate(newPlayerControllerPrefab, oldPlayer.position, oldPlayer.rotation, oldPlayer.parent);
            DontDestroyOnLoad(newPlayer);
            PlayerBasedManager.Player = newPlayer;

            Destroy(oldPlayer.gameObject);
        }
    }
}