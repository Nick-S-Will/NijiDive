using UnityEngine;

using NijiDive.Managers.Combo;
using NijiDive.Managers.Pausing;

namespace NijiDive.UI
{
    public class ComboTextUI : MonoBehaviour
    {
        [SerializeField] private TextUI comboText;
        [SerializeField] [Min(1)] private int comboVisibleThreshold = 5;

        private void Start()
        {
            ComboManager.singleton.OnCombo.AddListener(UpdateComboText);
            ComboManager.singleton.OnEndCombo.AddListener(ResetComboText);
        }

        private void UpdateComboText(int currentCombo)
        {
            if (currentCombo < comboVisibleThreshold) return;

            comboText.SetText(currentCombo.ToString());
            comboText.SetVisible(true);
        }

        private void ResetComboText(int finalCombo)
        {
            print(finalCombo);
            comboText.SetText("");
            comboText.SetVisible(false);
        }
    }
}