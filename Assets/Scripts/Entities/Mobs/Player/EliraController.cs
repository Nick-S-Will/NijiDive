using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls;
using NijiDive.Controls.Attacks;

namespace NijiDive.Entities.Mobs.Player
{
    public class EliraController : PlayerController
    {
        [Header("Special")]
        [SerializeField] private DiveBlast diveBlast;

        protected override void Awake()
        {
            controls = new List<Control> { diveBlast };

            base.Awake();
        }
    }
}