using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace NijiDive.UI.HUD
{
    public class BarHUD : TextHUD
    {
        [Space]
        [SerializeField] private SpriteRenderer backRenderer;
        [SerializeField] private SpriteRenderer foreRenderer;
        [SerializeField] [Min(0f)] private float barWidth = 1f, barHeight = 5f;
        [Space]
        [SerializeField] private Light2D barLight;

        public float GetBarFill => foreRenderer.transform.parent.localScale.y;
        public void SetBarFill(int amount, int max)
        {
            var percent = (float)amount / max;
            if (percent < 0f || percent > 1f) Debug.LogWarning("Bar fill set outside expected [0, 1] range", this);

            textMesh.text = amount.ToString();
            var scale = foreRenderer.transform.parent.localScale;
            scale.y = percent;
            foreRenderer.transform.parent.localScale = scale;
        }
        
        [ContextMenu("Update Shape")]
        public override void UpdateShape()
        {
            base.UpdateShape();

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

        public override void SetVisible(bool visible)
        {
            if (visibilityIsLocked) return;

            base.SetVisible(visible);

            backRenderer.enabled = visible;
            foreRenderer.enabled = visible;
            barLight.enabled = visible;

            base.SetVisible(visible);
        }
    }
}