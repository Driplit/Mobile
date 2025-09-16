using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    [Header("Resources")]
    [SerializeField] private int cash = 0; // resets each round
    [SerializeField] private int coins = 0; // persistent currency
    [SerializeField] private int gems = 0;  // premium currency

    [Header("UI References")]
    [SerializeField] private TMP_Text[] cashText;
    [SerializeField] private TMP_Text[] coinsText;
    [SerializeField] private TMP_Text[] gemsText;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    // --- Cash methods ---
    public int GetCash() => cash;
    public void AddCash(int amount)
    {
        cash += Mathf.Max(0, amount);
        UpdateUI();
    }
    public void ResetCash()
    {
        cash = 0;
        UpdateUI();
    }
    public bool SpendCash(int amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    // --- Coins methods ---
    public int GetCoins() => coins;
    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        UpdateUI();
    }
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    // --- Gems methods ---
    public int GetGems() => gems;
    public void AddGems(int amount)
    {
        gems += Mathf.Max(0, amount);
        UpdateUI();
    }
    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    // --- Update UI ---
    private void UpdateUI()
    {
        if (cashText != null)
        {
            foreach (var text in cashText)
                if (text != null) text.text = cash.ToString();
        }

        if (coinsText != null)
        {
            foreach (var text in coinsText)
                if (text != null) text.text = coins.ToString();
        }

        if (gemsText != null)
        {
            foreach (var text in gemsText)
                if (text != null) text.text = gems.ToString();
        }
    }
}
