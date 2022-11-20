using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace NijiDive.UI
{
    public class BarUI : MonoBehaviour // TODO: Create abstract parent class for UIs with a visibility toggle method
    {
        // TODO: Add number on bar
        [SerializeField] private SpriteRenderer backRenderer, foreRenderer;
        [SerializeField] [Min(0f)] private float barWidth = 1f, barHeight = 5f;
        [SerializeField] [Range(0f, 1f)] private float startBarFill = 1f;
        [Space]
        [SerializeField] private Light2D barLight;

        private void Start() => SetBarFill(startBarFill);
        
        [ContextMenu("Update Shape")]
        public void UpdateShape()
        {
            if (backRenderer == null || foreRenderer == null || barLight == null)
            {
                Debug.LogWarning($"{nameof(backRenderer)}, {nameof(foreRenderer)}, and {nameof(barLight)} must be assigned to update shape");
                return;
            }

            var size = new Vector2(barWidth, barHeight);
            backRenderer.size = size;
            foreRenderer.size = size;

            foreRenderer.transform.parent.localPosition = -size / 2f;
            foreRenderer.transform.localPosition = size / 2f;

            barLight.transform.localScale = size;
        }

        public float GetBarFill => foreRenderer.transform.parent.localScale.y;
        public void SetBarFill(float percent)
        {
            if (percent < 0f || percent > 1f) Debug.LogWarning("Bar fill set outside expected [0, 1] range", this);

            var scale = foreRenderer.transform.parent.localScale;
            scale.y = percent;
            foreRenderer.transform.parent.localScale = scale;
        }
    }
}