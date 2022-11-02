using UnityEngine;

using NijiDive.Controls;

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
        public void TryBounceDamageSource(GameObject sourceObject)
        {
            Debug.Log(sourceObject);
            var mob = sourceObject.GetComponent<Mob>();
            if (mob) mob.SetVelocityY(bounceSpeed);
        }
    }
}