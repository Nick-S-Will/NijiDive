using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.Managers.UI;
using NijiDive.Controls.UI;
using NijiDive.MenuItems;
using NijiDive.Utilities;

namespace NijiDive.UI
{
    public class UpgradeMenuUI : UIMenu
    {
        [Space]
        public UnityEvent OnSelect;
        [SerializeField] private Character[] characters;
        [SerializeField] private List<Upgrade> upgradeOptions;

        private Upgrade[] upgradesToPickFrom = new Upgrade[BASE_UPGRADE_COUNT];

        private static HashSet<Upgrade> equippedUpgrades = new HashSet<Upgrade>();
        private const int BASE_UPGRADE_COUNT = 3;

        private void Start()
        {
            AssignUpgradesToPickFrom();
            DisplayUpgradeSprites();

            SetMenuControls(true);
        }

        private void AssignUpgradesToPickFrom()
        {
            if (LevelManager.singleton.WorldIndex == 0)
            {
                upgradesToPickFrom = characters;
                return;
            }

            upgradesToPickFrom = new Upgrade[BASE_UPGRADE_COUNT];
            var shuffledUpgrades = upgradeOptions.ToArray();
            shuffledUpgrades.Shuffle();

            int index = 0;
            foreach (var upgrade in shuffledUpgrades)
            {
                if (equippedUpgrades.Contains(upgrade)) continue;

                upgradesToPickFrom[index] = upgrade;

                if (++index == upgradesToPickFrom.Length) break;
            }
        }

        private void UpdateSelectedGraphicsAndText()
        {
            UpdateSelectedGraphics();
            itemNameText.text = upgradesToPickFrom[SelectedIndex].name;
            itemDescriptionText.text = upgradesToPickFrom[SelectedIndex].Description;
        }

        private void DisplayUpgradeSprites()
        {
            for (int i = 0; i < itemSpriteRenderers.Length; i++)
            {
                var upgrade = upgradesToPickFrom[i];
                itemSpriteRenderers[i].sprite = upgrade.MenuSprite;
            }

            UpdateSelectedGraphicsAndText();
        }

        protected override void SetMenuControls(bool enabled)
        {
            if (UIManager.singleton.Player == null) return;

            var uiControl = UIManager.singleton.Player.GetControlType<UIControl>();
            uiControl.enabled = enabled;

            SetEventListeners(
                new UnityEvent[] { uiControl.OnSelect, uiControl.OnLeft, uiControl.OnRight },
                new UnityAction[] { Select, NavigateLeft, NavigateRight },
                enabled);
        }

        #region UI Control
        public override void Select()
        {
            var upgrade = upgradesToPickFrom[SelectedIndex];
            upgrade.Equip();
            equippedUpgrades.Add(upgrade);
            LevelManager.singleton.CompleteUpgrade();

            OnSelect?.Invoke();
        }

        private void Navigate(int indexDirection)
        {
            SelectedIndex = (SelectedIndex + indexDirection + BASE_UPGRADE_COUNT) % BASE_UPGRADE_COUNT;
            UpdateSelectedGraphicsAndText();

            OnNavigate?.Invoke();
        }
        public override void NavigateLeft() => Navigate(-1);
        public override void NavigateRight() => Navigate(1);
        #endregion

        protected override void OnValidate()
        {
            if (itemSpriteRenderers.Length != BASE_UPGRADE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(itemSpriteRenderers)} must be the same as {nameof(BASE_UPGRADE_COUNT)}");
                Array.Resize(ref itemSpriteRenderers, BASE_UPGRADE_COUNT);
            }

            base.OnValidate();
        }

        private void OnDestroy()
        {
            if (UIManager.singleton) SetMenuControls(false);
        }
    }
}