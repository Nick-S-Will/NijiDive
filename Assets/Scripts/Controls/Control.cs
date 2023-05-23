using System;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Entities.Mobs;

namespace NijiDive.Controls
{
    [Serializable]
    public abstract class Control
    {
        [HideInInspector] public UnityEvent OnReset = new UnityEvent();

        [HideInInspector] protected Mob mob;
        [HideInInspector] protected bool enabled = true;

        public bool IsEnabled => enabled;
        public void SetEnabled(bool enabled) => this.enabled = enabled;

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void Setup(Mob mob)
        {
            this.mob = mob;
            enabled = true;

            Awake();
            Start();
        }

        public abstract void TryToUse();

        public virtual void Reset() { OnReset.Invoke(); }

        public virtual void OnDestroy() { }
    }
}