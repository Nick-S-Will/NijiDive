using System;
using UnityEngine;

namespace NijiDive.UI
{
    public class TextUI : BaseUI
    {
        [SerializeField] protected TextMesh textMesh;
        [SerializeField] private SpriteRenderer textPanel;
        [SerializeField] private float panelBoarderSize = 0.25f;

        public string GetText()
        {
            return textMesh.text;
        }
        public void SetText(string text)
        {
            textMesh.text = text;
            UpdateShape();
        }

        [ContextMenu("Update Shape")]
        public override void UpdateShape()
        {
            textPanel.transform.localScale = (Vector2)textMesh.GetComponent<MeshRenderer>().bounds.size + (2f * panelBoarderSize * Vector2.one);
        }

        public override void SetVisible(bool visible)
        {
            if (visibilityIsLocked) return;

            textMesh.gameObject.SetActive(visible);
            if (textPanel) textPanel.gameObject.SetActive(visible);
        }
    }
}