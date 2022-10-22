using UnityEngine;

using NijiDive.Controls.Player;

namespace NijiDive.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BounceableTile")]
    public class BounceableTile : BreakableTile
    {
        [SerializeField] private float bounceSpeed = 2;

        public float BounceSpeed => bounceSpeed;

        /// <summary>
        /// Used by <see cref="BreakableTile.OnBreak"/> to bounce player by <see cref="bounceSpeed"/>
        /// </summary>
        public void BouncePlayer()
        {
            var player = FindObjectOfType<PlayerController>();
            player.SetVelocityY(bounceSpeed);
        }
    }
}