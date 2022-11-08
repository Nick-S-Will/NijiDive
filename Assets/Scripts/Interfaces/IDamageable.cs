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
        public bool TryDamage(GameObject sourceObject, int damage, DamageType damageType, Vector2 point);
    }

    /// <summary>
    /// Enum for the type of damage being inflicted
    /// </summary>
    [Flags]
    public enum DamageType
    {
        // Binary of enum makes up the damage type
        // Damage sources
        Enemy = 1 << 31,
        Player = 1 << 30,
        Environment = 1 << 29,

        // Damage types
        Suffocation = 1 << 3,
        Headbutt = 1 << 2,
        Stomp = 1 << 1,
        Projectile = 1
    }

    public static class DamageTypeMethods
    {
        public static bool IsVulnerableTo(this DamageType vulnerableTypes, DamageType inflictingTypes)
        {
            //Debug.Log($"\nvul: {Convert.ToString((int)vulnerableTypes, 2)}\ndam: {Convert.ToString((int)inflictingTypes, 2)}\nand: {Convert.ToString((int)(vulnerableTypes & inflictingTypes), 2)}");
            var count = BitwiseUtilities.BitCount((int)(vulnerableTypes & inflictingTypes));
            return count > 1;
        }
    }
}