using System.Collections;
using UnityEngine;

namespace NijiDive.UI
{
    public abstract class UI : MonoBehaviour
    {
        private Coroutine visibleRoutine;

        [HideInInspector] public bool visibilityIsLocked;

        [ContextMenu("Update Shape")]
        public virtual void UpdateShape() { }

        public abstract bool IsVisible { get; }
        public abstract void SetVisible(bool visible);
        [ContextMenu("Show UI")]
        public void ShowUI() => SetVisible(true);
        [ContextMenu("Hide UI")]
        public void HideUI() => SetVisible(false);

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