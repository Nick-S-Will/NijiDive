using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

using NijiDive.Managers.Persistence;
using NijiDive.Managers.Levels;
using NijiDive.Controls.Player;
using NijiDive.Items;
using NijiDive.Utilities;

public class UpgradeMenuUI : MonoBehaviour
{
    public UnityEvent OnNavigate, OnSelect;
    [SerializeField] private List<Upgrade> upgradeOptions;
    [SerializeField] private SpriteRenderer[] upgradeSlotRenderers = new SpriteRenderer[BASE_UPGRADE_COUNT];
    [SerializeField] private SpriteRenderer upgradeSelectorRenderer;
    [SerializeField] private TextMesh upgradeNameText, upgradeDescriptionText;
    [Space]
    [SerializeField] [Min(0f)] private float slotSpacing;

    private Upgrade[] upgradesToPickFrom = new Upgrade[BASE_UPGRADE_COUNT];
    private PlayerController player;
    private int selectedIndex = 0;

    private static HashSet<Upgrade> equippedUpgrades = new HashSet<Upgrade>();
    private const int BASE_UPGRADE_COUNT = 2;

    void Start()
    {
        player = PersistenceManager.FindPersistentObjectOfType<PlayerController>();
        GivePlayerUpgradeMenuControl();
        var camOffset = PersistenceManager.FindPersistentObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        player.transform.position = new Vector2(0.5f, -camOffset.y);

        AssignUpgradesToPickFrom();
        UpdateSlotPositions();
        DisplayUpgradeSprites();
    }

    #region Set Up Menu
    private void AssignUpgradesToPickFrom()
    {
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

    public void UpdateSlotPositions()
    {
        for (int i = 0; i < upgradeSlotRenderers.Length; i++)
        {
            var xPosition = (i * slotSpacing) - ((upgradeSlotRenderers.Length - 1) / 2f * slotSpacing);
            upgradeSlotRenderers[i].transform.localPosition = xPosition * Vector2.right;
        }

        upgradeSelectorRenderer.transform.position = upgradeSlotRenderers[0].transform.position;
    }

    private void DisplayUpgradeSprites()
    {
        for (int i = 0; i < upgradeSlotRenderers.Length; i++)
        {
            var upgrade = upgradesToPickFrom[i];
            upgradeSlotRenderers[i].sprite = upgrade.MenuSprite;
        }

        UpdateSelectedUpgradeGraphics();
    }
    #endregion

    private void UpdateSelectedUpgradeGraphics()
    {
        upgradeSelectorRenderer.transform.position = upgradeSlotRenderers[selectedIndex].transform.position;
        upgradeNameText.text = upgradesToPickFrom[selectedIndex].name;
        upgradeDescriptionText.text = upgradesToPickFrom[selectedIndex].Description;
    }

    #region Setting Upgrade Menu Controls
    private void GivePlayerUpgradeMenuControl()
    {
        var upgradeMenuControl = new UpgradeMenuControl();
        upgradeMenuControl.OnSelect.AddListener(Select);
        upgradeMenuControl.OnLeft.AddListener(NavigateLeft);
        upgradeMenuControl.OnRight.AddListener(NavigateRight);
        upgradeMenuControl.mob = player;
        upgradeMenuControl.enabled = true;

        player.AddControlType(upgradeMenuControl);
    }
    private void RemovePlayerUpgradeMenuControl()
    {
        if (player == null) return; // No error since player may have been destroyed

        var upgradeMenuControl = player.RemoveControlType<UpgradeMenuControl>();
        upgradeMenuControl.OnSelect.RemoveListener(Select);
        upgradeMenuControl.OnLeft.RemoveListener(NavigateLeft);
        upgradeMenuControl.OnRight.RemoveListener(NavigateRight);
    }
    #endregion

    #region UI Control
    private void Select()
    {
        var upgrade = upgradesToPickFrom[selectedIndex];
        upgrade.Equip();
        equippedUpgrades.Add(upgrade);
        LevelManager.singleton.CompleteUpgrade();

        OnSelect?.Invoke();
    }

    private void Navigate(int indexDirection)
    {
        selectedIndex = (selectedIndex + indexDirection + BASE_UPGRADE_COUNT) % BASE_UPGRADE_COUNT;
        UpdateSelectedUpgradeGraphics();

        OnNavigate?.Invoke();
    }
    public void NavigateLeft() => Navigate(-1);
    public void NavigateRight() => Navigate(1);
    #endregion

    private void OnValidate()
    {
        if (upgradeSlotRenderers.Length != BASE_UPGRADE_COUNT)
        {
            Debug.LogWarning($"The length of {nameof(upgradeSlotRenderers)} must be the same as {nameof(BASE_UPGRADE_COUNT)}");
            Array.Resize(ref upgradeSlotRenderers, BASE_UPGRADE_COUNT);
        }

        UpdateSlotPositions();
    }

    private void OnDestroy()
    {
        RemovePlayerUpgradeMenuControl();
    }
}