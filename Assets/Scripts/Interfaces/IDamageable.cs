using System;
using UnityEngine;

using NijiDive.Utilities;

namespace NijiDive
{
    public interface IDamageable
    {
        /// <summary>
        /// Checks if damage type affects this and inflicts it if so
        /// </summary>
        /// <param name="damage">Amount of damage inflicted</param>
        /// <param name="damageType"><see cref="DamageType"/> of the damage source</param>
        /// <param name="point">Point on the collider where the damage is taken</param>
        /// <returns>True if damage was inflicted on this</returns>
        public bool TryDamage(MonoBehaviour sourceBehaviour, int damage, DamageType damageType, Vector2 point);
    }

    /// <summary>
    /// Enum for the type of damage being inflicted
    /// </summary>
    [Flags]
    public enum DamageType
    {
        // Damage sources
        Enemy = 1 << 31,
        Player = 1 << 30,
        Environment = 1 << 29,

        // Damage types
        Void = 1 << 5,
        Shove = 1 << 4,
        Suffocation = 1 << 3,
        Headbutt = 1 << 2,
        Stomp = 1 << 1,
        Projectile = 1
    }

    public static class DamageTypeMethods
    {
        public static bool IsVulnerableTo(this DamageType vulnerableTypes, DamageType inflictingTypes)
        {
            var count = ((int)(vulnerableTypes & inflictingTypes)).GetBitCount();
            return count > 1;
        }
    }
}