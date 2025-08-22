using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShopUpgrade
{
    public StatType statType;       // Which stat this upgrade affects
    public float upgradeAmount;     // How much the stat increases per purchase
    public int cost;                // Cost in coins
    public int maxLevel = 1;        // Max purchase count, 0 means unlimited
    [HideInInspector] public int currentLevel = 0;  // Tracks how many times bought
}

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public TowerStats towerStats;
    public Wallet wallet; // Reference to the player's wallet


    [Header("Upgrades")]
    public List<ShopUpgrade> upgrades = new List<ShopUpgrade>();

    // Attempts to buy an upgrade by index
    public bool BuyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
        {
            Debug.LogWarning("Upgrade index out of range.");
            return false;
        }

        ShopUpgrade upgrade = upgrades[upgradeIndex];

        if (wallet.GetCoins() < upgrade.cost)
        {
            Debug.Log("Not enough coins to buy upgrade.");
            return false;
        }

        if (upgrade.maxLevel != 0 && upgrade.currentLevel >= upgrade.maxLevel)
        {
            Debug.Log("Upgrade max level reached.");
            return false;
        }

        // Deduct cost from Wallet
        wallet.SpendCoins(upgrade.cost);

        // Apply upgrade
        ApplyUpgrade(upgrade.statType, upgrade.upgradeAmount);

        upgrade.currentLevel++;

        Debug.Log($"Bought upgrade {upgrade.statType} (+{upgrade.upgradeAmount}). Level {upgrade.currentLevel}/{(upgrade.maxLevel == 0 ? "?" : upgrade.maxLevel.ToString())}");

        return true;
    }

    // Apply upgrade amount to the TowerStats stat
    private void ApplyUpgrade(StatType statType, float amount)
    {
        foreach (var stat in towerStats.attackStats)
        {
            if (stat.type == statType)
            {
                stat.value += amount;
                return;
            }
        }
        foreach (var stat in towerStats.defenceStats)
        {
            if (stat.type == statType)
            {
                stat.value += amount;
                return;
            }
        }
        foreach (var stat in towerStats.utilityStats)
        {
            if (stat.type == statType)
            {
                stat.value += amount;
                return;
            }
        }
    }

    // Get current level of an upgrade
    public int GetUpgradeLevel(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count)
            return 0;

        return upgrades[upgradeIndex].currentLevel;
    }

    // Optional: reset all upgrades (for testing)
    public void ResetUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = 0;
        }
        Debug.Log("All upgrades reset.");
    }
}
