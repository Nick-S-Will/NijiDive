using System;
using UnityEngine;

using NijiDive.Entities.Mobs;

namespace NijiDive.Controls
{
    [Serializable]
    public abstract class Control
    {
        [HideInInspector] protected Mob mob;
        [HideInInspector] protected bool enabled = true;

        public bool IsEnabled => enabled;
        public void SetEnabled(bool enabled) => this.enabled = enabled;

        public virtual void Awake() { }

        public virtual void Start() { }

        public void Setup(Mob mob)
        {
            this.mob = mob;
            enabled = true;

            Awake();
            Start();
        }

        public abstract void TryToUse();

        public virtual void Reset() { }

        public virtual void OnDestroy() { }
    }
}