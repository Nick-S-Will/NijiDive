using System.Collections.Generic;
using UnityEngine;

using NijiDive.Controls;
using NijiDive.Controls.Attacks.Specials;

namespace NijiDive.Entities.Mobs.Player
{
    public class PomuController : PlayerController
    {
        [Header("Special")]
        [SerializeField] private JumpBlast jumpBlast;

        protected override void Awake()
        {
            controls = new List<Control> { jumpBlast };

            base.Awake();
        }
    }
}