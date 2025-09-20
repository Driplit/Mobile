using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButtonUI : MonoBehaviour
{
    [Header("Setup")]
    public bool isPermanentShop = false;      // true = permanent (coins), false = in-game (cash)
    public StatType statType;                 // Which stat this button upgrades
    public UpgradeType upgradeType;           // Flat or Percent
    public int baseCost;                      // Base cost of first upgrade
    public int costGrowth;                    // Extra cost per level

    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text valueText;
    public TMP_Text costText;
    public Button buyButton;

    private Wallet wallet;
    private UpgradeManager upgradeManager;
    private PermanentUpgradesManager permanentManager;
    private TowerStats towerStats;

    private void Start()
    {
        wallet = Wallet.Instance;
        towerStats = FindFirstObjectByType<TowerStats>();
        upgradeManager = UpgradeManager.Instance;
        permanentManager = PermanentUpgradesManager.Instance;

        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuy);

        RefreshUI();
    }

    private int GetCurrentLevel()
    {
        if (isPermanentShop)
        {
            return permanentManager != null
                ? permanentManager.GetUpgradeLevel(statType, upgradeType)
                : 0;
        }
        else
        {
            return upgradeManager != null
                ? upgradeManager.GetUpgradeLevelByType(statType, upgradeType)
                : 0;
        }
    }

    private float GetCurrentStatValue()
    {
        if (towerStats == null) return 0f;
        return towerStats.GetStat(statType);
    }

    private int GetUpgradeCost()
    {
        int level = GetCurrentLevel();
        return baseCost + (level * costGrowth);
    }

    public void RefreshUI()
    {
        int level = GetCurrentLevel();
        float value = GetCurrentStatValue();
        int cost = GetUpgradeCost();

        nameText.text = $"{statType} ({upgradeType})";
        valueText.text = $"Value: {value:0.##} | Lvl {level}";
        costText.text = $"Cost: {cost} {(isPermanentShop ? "Coins" : "Cash")}";
    }

    private void OnBuy()
    {
        int cost = GetUpgradeCost();

        if (isPermanentShop)
        {
            if (!wallet.SpendCoins(cost))
            {
                Debug.Log("Not enough coins!");
                return;
            }

            // Upgrade the permanent stat
            permanentManager.UpgradeStat(statType, upgradeType, 1f);

            if (towerStats != null)
            {
                // Get the base value of the stat
                float baseStatValue = 0f;
                foreach (var stat in towerStats.attackStats)
                    if (stat.type == statType) baseStatValue = stat.baseValue;
                foreach (var stat in towerStats.defenceStats)
                    if (stat.type == statType) baseStatValue = stat.baseValue;
                foreach (var stat in towerStats.utilityStats)
                    if (stat.type == statType) baseStatValue = stat.baseValue;

                float oldValue = towerStats.GetStat(statType);
                float newValue = permanentManager.ApplyUpgrades(statType, baseStatValue);

                // If MaxHealth, add the difference to current health
                if (statType == StatType.MaxHealth)
                {
                    float difference = newValue - oldValue;
                    towerStats.Heal(difference);
                }

                towerStats.ApplyPermanentUpgrade(statType, newValue);
            }
        }
        else
        {
            if (!wallet.SpendCash(cost))
            {
                //Debug.Log("Not enough cash!");
                return;
            }

            // Apply in-game upgrade
            upgradeManager.BuyUpgrade(statType, upgradeType, wallet);
        }

        RefreshUI();
    }
}
