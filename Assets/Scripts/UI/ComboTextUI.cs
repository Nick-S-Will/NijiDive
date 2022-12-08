using UnityEngine;

using NijiDive.Managers.Combo;

namespace NijiDive.UI
{
    public class ComboTextUI : MonoBehaviour
    {
        [SerializeField] private TextUI comboText;
        [SerializeField] [Min(1)] private int comboVisibleThreshold = 5;
        [SerializeField] private string sortingLayerName = "Combo Text";

        private void Start()
        {
            ComboManager.singleton.OnCombo.AddListener(UpdateComboText);
            ComboManager.singleton.OnEndCombo.AddListener(ResetComboText);
        }

        [ContextMenu("Update Sorting Layer")]
        public void UpdateComboTextUISortingLayer()
        {
            // Layer meant to be between default and background layers
            comboText.TextMesh.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        }

        private void UpdateComboText(int currentCombo)
        {
            if (currentCombo < comboVisibleThreshold) return;

            comboText.SetText(currentCombo.ToString());
            comboText.SetVisible(true);
        }

        private void ResetComboText(int finalCombo)
        {
            comboText.SetText("");
            comboText.SetVisible(false);
        }
    }
}