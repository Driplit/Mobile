using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private int money = 0; // resets each round
    [SerializeField] private int coins = 0; // persistent currency
    [SerializeField] private int gems = 0;  // premium currency

    [Header("UI References")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text gemsText;

    private void Start()
    {
        UpdateUI();
    }

    // --- Money methods ---
    public int GetMoney() => money;
    public void AddMoney(int amount)
    {
        money += Mathf.Max(0, amount);
       // Debug.Log($"Money added: {amount}. Total money: {money}");
        UpdateUI();
    }
    public void ResetMoney()
    {
        money = 0;
        Debug.Log("Money reset to 0 at end of round.");
        UpdateUI();
    }

    // --- Coins methods ---
    public int GetCoins() => coins;
    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        //Debug.Log($"Coins added: {amount}. Total coins: {coins}");
        UpdateUI();
    }
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log($"Coins spent: {amount}. Coins left: {coins}");
            UpdateUI();
            return true;
        }
        Debug.Log("Not enough coins.");
        return false;
    }

    // --- Gems methods ---
    public int GetGems() => gems;
    public void AddGems(int amount)
    {
        gems += Mathf.Max(0, amount);
        Debug.Log($"Gems added: {amount}. Total gems: {gems}");
        UpdateUI();
    }
    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            Debug.Log($"Gems spent: {amount}. Gems left: {gems}");
            UpdateUI();
            return true;
        }
        Debug.Log("Not enough gems.");
        return false;
    }

    // --- Update UI ---
    private void UpdateUI()
    {
        if (moneyText != null) moneyText.text = $"Money: {money}";
        if (coinsText != null) coinsText.text = $"Coins: {coins}";
        if (gemsText != null) gemsText.text = $"Gems: {gems}";
    }
}
