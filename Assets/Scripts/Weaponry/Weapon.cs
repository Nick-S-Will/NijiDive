using UnityEngine;

using NijiDive.Entities.Contact;

namespace NijiDive.Weaponry
{
    [CreateAssetMenu(menuName = "NijiDive/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private Projectile projectile;
        [SerializeField] [Range(0f, 1f)] private float accuracy = 0.95f;
        [SerializeField] [Min(0f)] private float projectileSpeed = 10f, recoilSpeed = 5f, shotInterval = 0.5f, burstProjectileInterval = 0f;
        [SerializeField] [Min(1)] private int projectilesPerVolley = 1, projectilesPerBurst = 1, clipSize = 5;
        [SerializeField] private bool isAutomatic = true;

        private int leftInClip;

        public Projectile Projectile => projectile;
        /// <summary>
        /// Accuracy of the weapon which when at 0 allows up to 90 degrees off
        /// </summary>
        public float Accuracy => accuracy;
        public float ProjectileSpeed => projectileSpeed;
        public float RecoilSpeed => recoilSpeed;
        /// <summary>
        /// Time between each group of projectiles
        /// </summary>
        public float ShotInterval => shotInterval;
        /// <summary>
        /// Time between each projectile, used for bursts
        /// </summary>
        public float BurstProjectileInterval => burstProjectileInterval;
        public int ProjectilesPerVolley => projectilesPerVolley;
        public int ProjectilesPerBurst => projectilesPerBurst;
        /// <summary>
        /// Total number of projectiles able to be shot before needing a reload
        /// </summary>
        public int ClipSize => clipSize;
        /// <summary>
        /// Current number of projectiles able to be shot before reload
        /// </summary>
        public int LeftInClip => leftInClip;
        /// <summary>
        /// Controls if you can hold to shoot or need to press for each shot
        /// </summary>
        public bool IsAutomatic => isAutomatic;
        public bool ClipIsEmpty => LeftInClip <= 0;

        private float lastShotTime;

        /// <returns>True if <see cref="shotInterval"/> seconds have elapsed since <see cref="lastShotTime"/></returns>
        public bool CanShoot() => Time.time - lastShotTime >= shotInterval;

        public void Load()
        {
            lastShotTime = 0f;
            Reload();
        }

        /// <summary>
        /// Updates <see cref="leftInClip"/>
        /// </summary>
        public void CompleteVolley()
        {
            lastShotTime = Time.time;
            leftInClip--;
        }

        public void Reload()
        {
            leftInClip = clipSize;
        }
    }
}