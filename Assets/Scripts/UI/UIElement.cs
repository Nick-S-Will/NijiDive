using System.Collections;
using UnityEngine;

using NijiDive.Managers.Pausing;

namespace NijiDive.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        private Coroutine visibleRoutine;

        [HideInInspector] public bool visibilityIsLocked;

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

            yield return new PauseManager.WaitWhilePausedAndForSeconds(duration);

            SetVisible(false);
            visibleRoutine = null;
        }

        public Coroutine SetVisible(float duration)
        {
            if (visibilityIsLocked) return null;

            if (visibleRoutine != null) StopCoroutine(visibleRoutine);
            
            visibleRoutine = StartCoroutine(SetVisibleRoutine(duration));

            return visibleRoutine;
        }
    }
}