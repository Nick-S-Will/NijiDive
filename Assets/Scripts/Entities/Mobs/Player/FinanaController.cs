using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls;
using NijiDive.Controls.Attacks.Specials;

namespace NijiDive.Entities.Mobs.Player
{
    public class FinanaController : PlayerController
    {
        [Header("Special")]
        [SerializeField] private ZoneBlast zoneBlast;

        protected override void Awake()
        {
            controls = new List<Control> { zoneBlast };

            base.Awake();
        }
    }
}