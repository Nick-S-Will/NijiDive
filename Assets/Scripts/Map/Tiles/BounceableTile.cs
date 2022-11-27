using UnityEngine;

namespace NijiDive.Map.Tiles
{
    [CreateAssetMenu(menuName = "NijiDive/Tiles/BounceableTile")]
    public class BounceableTile : BreakableTile
    {
        [SerializeField] private float bounceSpeed = 5f;

        public float BounceSpeed => bounceSpeed;

        /// <summary>
        /// Used by <see cref="BreakableTile.OnBreak"/> to bounce <paramref name="sourceObject"/> by <see cref="bounceSpeed"/>
        /// </summary>
        public void TryBounceDamageSource(GameObject sourceObject)
        {
            var bounceable = sourceObject.GetComponent<IBounceable>();
            if (bounceable != null) bounceable.Bounce(bounceSpeed);
        }
    }
}