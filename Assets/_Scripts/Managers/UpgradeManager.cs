using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType { Flat, Percent }

[System.Serializable]
public class ShopUpgrade
{
    public StatType statType;
    public UpgradeType upgradeType;
    public float upgradeAmount;
    public int cost;
    public int maxLevel = 1;
    [HideInInspector] public int currentLevel = 0;
    public bool isPermanent = false;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    public TowerStats towerStats;

    public List<ShopUpgrade> upgrades = new List<ShopUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool BuyUpgrade(StatType statType, UpgradeType upgradeType, Wallet walletToUse)
    {
        ShopUpgrade upgrade = upgrades.Find(u => u.statType == statType && u.upgradeType == upgradeType);
        if (upgrade == null) return false;

        if (walletToUse.GetCash() < upgrade.cost) return false;

        walletToUse.SpendCash(upgrade.cost);

        // Apply upgrade
        if (towerStats != null)
        {
            ApplyUpgrade(towerStats, upgrade.statType, upgrade.upgradeType, upgrade.upgradeAmount);

            // If permanent, update tower base stats immediately
            if (upgrade.isPermanent)
            {
                float newValue = towerStats.GetStat(upgrade.statType);
                towerStats.ApplyPermanentUpgrade(upgrade.statType, newValue);

                // Update PermanentUpgradesManager
                PermanentUpgradesManager.Instance.UpgradeStat(upgrade.statType, upgrade.upgradeType, upgrade.upgradeAmount);
            }
        }

        upgrade.currentLevel++;
        return true;
    }

    private void ApplyUpgrade(TowerStats tower, StatType statType, UpgradeType type, float amount)
    {
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
                        stat.value *= 1f + amount;

                    return;
                }
            }
        }
    }
    // --- Get current level of a specific upgrade ---
    public int GetUpgradeLevelByType(StatType statType, UpgradeType upgradeType)
    {
        var upgrade = upgrades.Find(u => u.statType == statType && u.upgradeType == upgradeType);
        return upgrade != null ? upgrade.currentLevel : 0;
    }

}
