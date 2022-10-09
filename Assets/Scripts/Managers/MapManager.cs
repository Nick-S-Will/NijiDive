using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive.Managers
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;

        /// <summary>
        /// <see cref="LayerMask"/> used for terrain collisions
        /// </summary>
        public LayerMask GroundMask => groundMask;
    }
}