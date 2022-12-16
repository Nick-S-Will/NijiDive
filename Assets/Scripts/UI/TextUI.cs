using System;
using UnityEngine;

namespace NijiDive.UI
{
    public class TextUI : UIBase
    {
        [SerializeField] private SpriteRenderer textPanel;
        [SerializeField] protected TextMesh textMesh;
        [SerializeField] private float panelBoarderSize = 0.25f;

        public TextMesh TextMesh => textMesh;

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
            if (textPanel) textPanel.transform.localScale = (Vector2)textMesh.GetComponent<MeshRenderer>().bounds.size + (2f * panelBoarderSize * Vector2.one);
        }

        public override bool IsVisible => textMesh.gameObject.activeSelf;
        public override void SetVisible(bool visible)
        {
            if (visibilityIsLocked) return;

            if (textPanel) textPanel.gameObject.SetActive(visible);
            textMesh.gameObject.SetActive(visible);
        }
    }
}