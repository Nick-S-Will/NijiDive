using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NijiDive.Managers.Levels;
using NijiDive.Managers.PlayerBased;
using NijiDive.Managers.PlayerBased.UI;
using NijiDive.Controls.UI;
using NijiDive.MenuItems.Upgrades;
using NijiDive.Utilities;

namespace NijiDive.UI.Menu
{
    public class UpgradeMenuUI : UIItemStore
    {
        [Space]
        [SerializeField] private Vector2 playerPosition;
        [SerializeField] private Character[] characters;
        [SerializeField] private List<Upgrade> upgradeOptions;

        private Upgrade[] upgradesToPickFrom = new Upgrade[BASE_UPGRADE_COUNT];

        private static HashSet<Upgrade> equippedUpgrades = new HashSet<Upgrade>();
        private const int BASE_UPGRADE_COUNT = 3;

        protected override void Start()
        {
            base.Start();

            AssignUpgradesToPickFrom();
            UpdateMenuItemSprites(upgradesToPickFrom);

            PlayerBasedManager.Player.transform.position = playerPosition;

            SetMenuControls(true);
            OnOpen?.Invoke();
        }

        private void AssignUpgradesToPickFrom()
        {
            if (LevelManager.WorldIndex == 0)
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

        protected override void SetMenuControls(bool enabled)
        {
            var uiControl = PlayerBasedManager.Player.GetControlType<UIControl>();

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
            OnPurchase?.Invoke(upgrade);

            LevelManager.singleton.StartNextLevel();
        }

        private void Navigate(int indexDirection)
        {
            SelectedIndex = (SelectedIndex + indexDirection + BASE_UPGRADE_COUNT) % BASE_UPGRADE_COUNT;
            UpdateSelectedGraphicsAndText(upgradesToPickFrom[SelectedIndex]);

            OnNavigate?.Invoke();
        }
        public override void NavigateLeft() => Navigate(-1);
        public override void NavigateRight() => Navigate(1);
        #endregion

        protected override void OnValidate()
        {
            var optionTransforms = GetOptionsTransforms();
            if (optionTransforms.Length != BASE_UPGRADE_COUNT)
            {
                Debug.LogWarning($"The length of {nameof(optionTransforms)} must be the same as {nameof(BASE_UPGRADE_COUNT)}");
            }

            base.OnValidate();
        }

        private void OnDestroy()
        {
            if (UIManager.singleton) SetMenuControls(false);
            OnClose?.Invoke();
        }
    }
}