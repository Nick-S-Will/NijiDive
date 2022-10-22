using System;
using UnityEngine;

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
        public bool TakeDamage(int damage, DamageType damageType, Vector2 point);
    }

    /// <summary>
    /// Enum for the type of damage being inflicted
    /// </summary>
    [Flags]
    public enum DamageType
    {
        // Last 6 binary make up the damage type
        // Damage sources
        Enemy = 32,
        Player = 16,
        Environment = 8,

        // Damage types
        Suffocation = 4,
        Contact = 2,
        Projectile = 1
    }

    public static class DamageTypeMethods
    {
        public static bool IsVulnerableTo(this DamageType vulnerableTypes, DamageType inflictingTypes)
        {
            return vulnerableTypes == 0 ? false : Mathf.Log((float)(vulnerableTypes & inflictingTypes), 2f) % 1 != 0;
        }
    }
}