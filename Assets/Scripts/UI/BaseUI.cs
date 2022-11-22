using System.Collections;
using UnityEngine;

namespace NijiDive.UI
{
    public abstract class BaseUI : MonoBehaviour
    {
        private Coroutine visibleRoutine;

        [HideInInspector] public bool visibilityIsLocked;

        public abstract void UpdateShape();

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