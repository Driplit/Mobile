using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType
{
    Flat,
    Percent
}

[System.Serializable]
public class ShopUpgrade
{
    public StatType statType;           // Which stat this upgrade affects
    public UpgradeType upgradeType;     // Flat or Percent
    public float upgradeAmount;         // Amount per purchase (flat value or percent multiplier)
    public int cost;                    // Cost in cash
    public int maxLevel = 1;            // 0 = unlimited
    [HideInInspector] public int currentLevel = 0; // Tracks how many times bought
    public bool isPermanent = false;    // Determines if upgrade persists across runs
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("References")]
    public TowerStats towerStats; // Assign in inspector
    public Wallet wallet;         // Assign in inspector

    [Header("Upgrades")]
    public List<ShopUpgrade> upgrades = new List<ShopUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // important
    }


    // --- Buy an upgrade by index ---
    public bool BuyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
        {
            Debug.LogWarning("Upgrade index out of range.");
            return false;
        }

        ShopUpgrade upgrade = upgrades[upgradeIndex];
        return BuyUpgrade(upgrade.statType, upgrade.upgradeType, wallet);
    }

    // --- Buy upgrade by StatType + UpgradeType ---
    public bool BuyUpgrade(StatType statType, UpgradeType upgradeType, Wallet walletToUse)
    {
        ShopUpgrade upgrade = upgrades.Find(u => u.statType == statType && u.upgradeType == upgradeType);
        if (upgrade == null)
        {
            Debug.LogWarning($"Upgrade not found: {statType} ({upgradeType})");
            return false;
        }

        if (walletToUse == null)
        {
            Debug.LogWarning("Wallet reference is null!");
            return false;
        }

        // Check if player has enough currency
        if (walletToUse.GetCash() < upgrade.cost)
        {
            Debug.Log("Not enough currency!");
            return false;
        }

        // Spend the currency
        walletToUse.SpendCash(upgrade.cost);

        // Apply upgrade to tower stats
        if (towerStats != null)
            ApplyUpgrade(towerStats, upgrade.statType, upgrade.upgradeType, upgrade.upgradeAmount);
        else
            Debug.LogWarning("TowerStats reference is missing!");

        // Track upgrade level
        upgrade.currentLevel++;

        Debug.Log($"Bought {upgrade.statType} [{upgrade.upgradeType}] +{upgrade.upgradeAmount} | Level {upgrade.currentLevel}");

        return true;
    }

    // --- Apply upgrade to TowerStats ---
    private void ApplyUpgrade(TowerStats tower, StatType statType, UpgradeType type, float amount)
    {
        if (tower == null) return;

        List<Stat>[] statLists = { tower.attackStats, tower.defenceStats, tower.utilityStats };

        foreach (var list in statLists)
        {
            foreach (var stat in list)
            {
                if (stat.type == statType)
                {
                    if (type == UpgradeType.Flat)
                        stat.value += amount;
                    else if (type == UpgradeType.Percent)
                        stat.value *= (1f + amount);

                    return;
                }
            }
        }
    }

    // --- Get current level by index ---
    public int GetUpgradeLevel(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
            return 0;

        return upgrades[upgradeIndex].currentLevel;
    }

    // --- Get current level by StatType + UpgradeType ---
    public int GetUpgradeLevelByType(StatType statType, UpgradeType upgradeType)
    {
        var upgrade = upgrades.Find(u => u.statType == statType && u.upgradeType == upgradeType);
        return upgrade != null ? upgrade.currentLevel : 0;
    }

    // --- Get current stat value from TowerStats ---
    public float GetStatValue(StatType statType)
    {
        if (towerStats == null) return 0f;

        List<Stat>[] statLists = { towerStats.attackStats, towerStats.defenceStats, towerStats.utilityStats };

        foreach (var list in statLists)
        {
            foreach (var stat in list)
            {
                if (stat.type == statType)
                    return stat.value;
            }
        }

        return 0f;
    }

    // --- Reset all upgrades ---
    public void ResetUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = 0;
        }
        Debug.Log("All upgrades reset.");
    }
}
