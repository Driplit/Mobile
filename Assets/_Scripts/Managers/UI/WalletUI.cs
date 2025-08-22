using TMPro;
using UnityEngine;
public class WalletUI : MonoBehaviour
{
    [Header("References")]
    public Wallet wallet;

    [Header("UI Elements")]
    public TMP_Text moneyText;
    public TMP_Text coinsText;
    public TMP_Text gemsText;

    private void Update()
    {
        // Update UI each frame (you can optimize with events later)
        moneyText.text = $"Money: {wallet.GetMoney()}";
        coinsText.text = $"Coins: {wallet.GetCoins()}";
        gemsText.text = $"Gems: {wallet.GetGems()}";
    }
}