using UnityEngine;

using NijiDive.Managers.PlayerBased.Combo;

namespace NijiDive.UI.HUD
{
    public class ComboTextHUD : MonoBehaviour
    {
        [SerializeField] private TextHUD comboText;
        [SerializeField] [Min(1)] private int comboVisibleThreshold = 5;
        [SerializeField] private string sortingLayerName = "Combo Text";

        private void OnEnable()
        {
            ComboManager.OnCombo.AddListener(UpdateComboText);
            ComboManager.OnEndCombo.AddListener(ResetComboText);
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

        private void OnDisable()
        {
            ComboManager.OnCombo.RemoveListener(UpdateComboText);
            ComboManager.OnEndCombo.RemoveListener(ResetComboText);
        }
    }
}