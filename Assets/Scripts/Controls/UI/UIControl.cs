using NijiDive.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace NijiDive.Controls.UI
{
    public class UIControl : Control
    {
        public UnityEvent OnSelect, OnCancel, OnLeft, OnRight, OnDown, OnUp;

        private Vector2 lastDirection;

        public UIControl(Mob mob = null, bool enabled = false)
        {
            OnSelect = new UnityEvent();
            OnCancel = new UnityEvent();
            OnLeft = new UnityEvent();
            OnRight = new UnityEvent();
            OnDown = new UnityEvent();
            OnUp = new UnityEvent();

            this.mob = mob;
            this.enabled = enabled;
            lastDirection = Vector2.zero;
        }

        public override void Use()
        {
            if (mob.LastInputs.actionDownThisFrame) OnSelect?.Invoke();
            if (mob.LastInputs.altDownThisFrame) OnCancel?.Invoke();

            var direction = mob.LastInputs.lStick;
            if (direction.x != 0f && direction.x * lastDirection.x <= 0f) (direction.x > 0f ? OnRight : OnLeft).Invoke();
            if (direction.y != 0f && direction.y * lastDirection.y <= 0f) (direction.y > 0 ? OnUp : OnDown).Invoke();

            lastDirection = direction;
        }
    }
}