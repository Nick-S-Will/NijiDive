using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive
{
    public interface IDamageable
    {
        /// <summary>
        /// Enum for the type of damage being inflicted
        /// </summary>
        public enum DamageType { Player, Mob, Environment }

        /// <summary>
        /// Checks if damage type affects this and inflicts it if so
        /// </summary>
        /// <param name="damage">Amount of damage inflicted</param>
        /// <param name="damageType"><see cref="DamageType"/> of the damage source</param>
        /// <param name="point">Point on the collider where the damage is taken</param>
        /// <returns>True if damage was inflicted on this</returns>
        public bool TakeDamage(int damage, DamageType damageType, Vector2 point);
    }
}