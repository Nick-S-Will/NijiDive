using UnityEngine;

namespace NijiDive.Entities
{
    public abstract class Entity : MonoBehaviour, IPauseable
    {
        public virtual bool IsPaused { get; protected set; }

        public virtual void SetPaused(bool paused) { }
    }
}