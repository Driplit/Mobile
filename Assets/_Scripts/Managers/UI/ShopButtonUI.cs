using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButtonUI : MonoBehaviour
{
    [Header("Setup")]
    public bool isPermanentShop = false;  // true = coins (main menu), false = cash (in-game)
    public StatType statType;             // which stat this button upgrades
    public UpgradeType upgradeType;       // flat or percent
    public int baseCost = 50;             // cost of first upgrade
    public int costGrowth = 20;           // extra cost per level

    [Header("UI References")]
    public TMP_Text nameText;             // Upgrade name
    public TMP_Text valueText;            // Current value
    public TMP_Text costText;             // Cost with currency
    public Button buyButton;              // Button to buy

    private Wallet wallet;
    private UpgradeManager upgradeManager;
    private PermanentUpgradesManager permanentManager;

    private void Start()
    {
        wallet = Wallet.Instance;

        // Delay initialization if needed
        if (isPermanentShop && PermanentUpgradesManager.Instance == null)
        {
            Debug.Log("PermanentUpgradesManager not ready, delaying setup");
            Invoke(nameof(InitializeButton), 0.1f);
        }
        else if (!isPermanentShop && UpgradeManager.Instance == null)
        {
            Debug.Log("UpgradeManager not ready, delaying setup");
            Invoke(nameof(InitializeButton), 0.1f);
        }
        else
        {
            InitializeButton();
        }
    }

    private void InitializeButton()
    {
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
            if (permanentManager == null) return 0; // safe fallback
            return permanentManager.GetUpgradeLevel(statType, upgradeType);
        }
        else
        {
            if (upgradeManager == null) return 0; // safe fallback
            return upgradeManager.GetUpgradeLevelByType(statType, upgradeType);
        }
    }

    private float GetCurrentStatValue()
    {
        return isPermanentShop
            ? permanentManager.ApplyUpgrades(statType, 1f)
            : upgradeManager.GetStatValue(statType);
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
            permanentManager.UpgradeStat(statType, upgradeType, 1f);
        }
        else
        {
            if (!wallet.SpendCash(cost))
            {
                Debug.Log("Not enough cash!");
                return;
            }
            upgradeManager.BuyUpgrade(statType, upgradeType, wallet);
        }

        RefreshUI();
    }
}
