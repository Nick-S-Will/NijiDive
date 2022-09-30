using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NijiDive.Managers
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private LayerMask groundMask;

        public LayerMask GroundMask => groundMask;
    }
}