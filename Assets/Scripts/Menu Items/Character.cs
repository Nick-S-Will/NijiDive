using UnityEngine;

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

            var oldPlayer = GameObject.FindWithTag(Constants.PLAYER_TAG);
            _ = Instantiate(newPlayerControllerPrefab, oldPlayer.transform.position, oldPlayer.transform.rotation, oldPlayer.transform.parent);
            Destroy(oldPlayer);
        }
    }
}