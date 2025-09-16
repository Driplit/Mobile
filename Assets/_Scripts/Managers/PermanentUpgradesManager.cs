using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PermanentUpgrade
{
    public StatType statType;
    public UpgradeType upgradeType;
    public float upgradeStep;
    public int level;
}

public class PermanentUpgradesManager : MonoBehaviour
{
    public static PermanentUpgradesManager Instance { get; private set; }

    [SerializeField] private List<PermanentUpgrade> upgrades = new List<PermanentUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitDefaults();
        LoadUpgrades();
    }

    private void InitDefaults()
    {
        if (upgrades.Count == 0)
        {
            upgrades.Add(new PermanentUpgrade { statType = StatType.MaxHealth, upgradeType = UpgradeType.Flat, upgradeStep = 5f, level = 0 });
            upgrades.Add(new PermanentUpgrade { statType = StatType.Damage, upgradeType = UpgradeType.Percent, upgradeStep = 0.1f, level = 0 });
            upgrades.Add(new PermanentUpgrade { statType = StatType.AttackSpeed, upgradeType = UpgradeType.Flat, upgradeStep = 0.5f, level = 0 });
        }
    }

    public float ApplyUpgrades(StatType type, float baseValue)
    {
        float result = baseValue;

        foreach (var upgrade in upgrades)
        {
            if (upgrade.statType == type && upgrade.level > 0)
            {
                if (upgrade.upgradeType == UpgradeType.Flat)
                    result += upgrade.upgradeStep * upgrade.level;
                else if (upgrade.upgradeType == UpgradeType.Percent)
                    result *= 1f + (upgrade.upgradeStep * upgrade.level);
            }
        }

        return result;
    }

    public void UpgradeStat(StatType type, UpgradeType upgradeType, float stepValue)
    {
        var upgrade = upgrades.Find(u => u.statType == type && u.upgradeType == upgradeType);
        if (upgrade == null)
        {
            upgrade = new PermanentUpgrade { statType = type, upgradeType = upgradeType, upgradeStep = stepValue, level = 0 };
            upgrades.Add(upgrade);
        }

        upgrade.level++;
        SaveUpgrades();
    }

    public int GetUpgradeLevel(StatType type, UpgradeType upgradeType)
    {
        var upgrade = upgrades.Find(u => u.statType == type && u.upgradeType == upgradeType);
        return upgrade != null ? upgrade.level : 0;
    }

    private void SaveUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            string key = $"PermUpgrade_{upgrade.statType}_{upgrade.upgradeType}";
            PlayerPrefs.SetInt(key, upgrade.level);
        }
        PlayerPrefs.Save();
    }

    private void LoadUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            //string key = $"PermUpgrade_{upgrade.statType}_{upgrade.upgradeType}";
            //// Only load saved levels if PlayerPrefs has a key
            //if (PlayerPrefs.HasKey(key))
            //    upgrade.level = PlayerPrefs.GetInt(key, 0);
            //else
            upgrade.level = 0; // fresh install = no upgrades
        }
    }

    public void ResetUpgrades()
    {
        foreach (var upgrade in upgrades)
            upgrade.level = 0;

        PlayerPrefs.DeleteAll();
    }
}
