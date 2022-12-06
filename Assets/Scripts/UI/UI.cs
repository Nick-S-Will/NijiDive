using System.Collections;
using UnityEngine;

namespace NijiDive.UI
{
    public abstract class UI : MonoBehaviour
    {
        private Coroutine visibleRoutine;

        [HideInInspector] public bool visibilityIsLocked;

        public virtual void UpdateShape() { }

        public abstract bool IsVisible { get; }
        public abstract void SetVisible(bool visible);

        private IEnumerator SetVisibleRoutine(float duration)
        {
            SetVisible(true);

            yield return new WaitForSeconds(duration);

            SetVisible(false);
            visibleRoutine = null;
        }

        public void SetVisible(float duration)
        {
            if (visibilityIsLocked) return;

            if (visibleRoutine != null) StopCoroutine(visibleRoutine);
            
            visibleRoutine = StartCoroutine(SetVisibleRoutine(duration));
        }
    }
}