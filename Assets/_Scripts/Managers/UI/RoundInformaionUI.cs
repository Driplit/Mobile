using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundInformationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveSpawner waveSpawner; // Reference to the WaveSpawner for current round and timer
    [SerializeField] private TowerStats towerStats;   // Reference to the TowerStats for player stats
    [SerializeField] private Enemy enemyPrefab;       // Reference to the enemy prefab for displaying enemy stats

    // ----- Round Info -----
    [Header("UI Round Stats")]
    [SerializeField] private TMP_Text currentRoundText;
    [SerializeField] private TMP_Text currentRoundTierText;
    [SerializeField] private Slider roundTimerSlider;
    [SerializeField] private TMP_Text currentBasicEnemyDamageText;
    [SerializeField] private TMP_Text currentBasicEnemyHealthText;

    // ----- Player Info -----
    [Header("UI Player Stats")]
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text playerMaxHealthText;
    [SerializeField] private TMP_Text playerDamageText;
    [SerializeField] private TMP_Text playerHealingText;
    [SerializeField] private TMP_Text coinMultiplierText;
    [SerializeField] private Slider playerHealthSlider;

    
    private void Update()
    {
        // Try to get waveSpawner if it's null (scene changed)
        if (waveSpawner == null)
        {
            waveSpawner = FindFirstObjectByType<WaveSpawner>();
        }
        if (towerStats == null)
        {
           towerStats = FindFirstObjectByType<TowerStats>();
        }

        UpdateRoundInfo();
        UpdatePlayerInfo();
    }

    private void UpdateRoundInfo()
    {
        if (waveSpawner != null)
        {
            currentRoundText.text = $"Round: {waveSpawner.CurrentRound}";
            currentRoundTierText.text = $"Tier: {waveSpawner.CurrentTier}";

            if (roundTimerSlider != null)
            {
                roundTimerSlider.maxValue = waveSpawner.RoundDuration;
                roundTimerSlider.value = waveSpawner.CurrentRoundTimer;
            }

            if (enemyPrefab != null && currentBasicEnemyDamageText != null && currentBasicEnemyHealthText != null)
            {
                var (health, damage) = enemyPrefab.GetPreviewStats(waveSpawner.CurrentRound);
                currentBasicEnemyDamageText.text = $"D: {damage:F2}";
                currentBasicEnemyHealthText.text = $"H: {health:F2}";
            }
        }
    }

    private void UpdatePlayerInfo()
    {
        if (towerStats == null) return;

        // Health
        if (playerHealthText != null) playerHealthText.text = $"{towerStats.CurrentHealth}";
        if (playerMaxHealthText != null) playerMaxHealthText.text = $"/ {towerStats.MaxHealth}";
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = towerStats.MaxHealth;
            playerHealthSlider.value = towerStats.CurrentHealth;
        }

        // Attack
        if (playerDamageText != null) playerDamageText.text = $"D: {towerStats.Damage}";
        if (playerHealingText != null) playerHealingText.text = $"H: {towerStats.HealthRegen}";

        // Utility
        if (coinMultiplierText != null) coinMultiplierText.text = $"C: {towerStats.CashMultiplier}x";
    }
}
