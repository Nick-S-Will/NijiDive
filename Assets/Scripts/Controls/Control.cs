using System;
using UnityEngine;

using NijiDive.Entities;

namespace NijiDive.Controls
{
    [Serializable]
    public abstract class Control
    {
        [HideInInspector] public Mob mob;
        [HideInInspector] public bool enabled = true;

        public virtual void Awake() { }

        public virtual void Start() { }

        public abstract void Use();
    }
}