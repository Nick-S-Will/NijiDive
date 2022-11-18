using UnityEngine.Events;

namespace NijiDive.Controls.Player
{
    public class ShopControl : Control
    {
        public UnityEvent OnSelect, OnCancel, OnLeft, OnRight;

        private float lastDir;

        public ShopControl()
        {
            OnSelect = new UnityEvent();
            OnCancel = new UnityEvent();
            OnLeft = new UnityEvent();
            OnRight = new UnityEvent();
            lastDir = 0;
        }

        public override void FixedUpdate()
        {
            if (mob.LastInputs.actionDownThisFrame) OnSelect?.Invoke();
            if (mob.LastInputs.altDownThisFrame) OnCancel?.Invoke();

            var xDir = mob.LastInputs.lStick.x;
            if (xDir < 0f && lastDir >= 0f) OnLeft?.Invoke();
            else if (xDir > 0f && lastDir <= 0f) OnRight?.Invoke();

            lastDir = xDir;
        }
    }
}