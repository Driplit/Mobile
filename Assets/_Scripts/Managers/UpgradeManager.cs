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

        if (towerStats != null)
        {
            // Store old value
            float oldValue = towerStats.GetStat(statType);

            // Apply upgrade
            ApplyUpgrade(towerStats, upgrade.statType, upgrade.upgradeType, upgrade.upgradeAmount);

            float newValue = towerStats.GetStat(statType);

            // If MaxHealth, add difference to current health
            if (statType == StatType.MaxHealth)
            {
                float difference = newValue - oldValue;
                towerStats.Heal(difference);
            }

            // If permanent (should not happen here normally), also update PermanentUpgradesManager
            if (upgrade.isPermanent)
            {
                towerStats.ApplyPermanentUpgrade(statType, newValue);
                PermanentUpgradesManager.Instance.UpgradeStat(statType, upgrade.upgradeType, upgrade.upgradeAmount);
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
