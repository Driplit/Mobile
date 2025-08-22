using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundInformaionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveSpawner waveSpawner; // Reference to the WaveSpawner for current round and timer
    [SerializeField] private TowerStats towerStats;   // Reference to the TowerStats for player stats
    [SerializeField] private Enemy enemyPrefab;       // Reference to the enemy prefab for displaying enemy stats

    // ----- Round Info -----
    [Header("UI Round Stats")]
    [SerializeField] private TextMeshProUGUI currentRoundText;
    [SerializeField] private TextMeshProUGUI currentRoundTierText;
    [SerializeField] private Slider roundTimerSlider;
    [SerializeField] private TextMeshProUGUI currentBasicEnemyDamageText;
    [SerializeField] private TextMeshProUGUI currentBasicEnemyHealthText;

    // ----- Player Info -----
    [Header("UI Player Stats")]
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI playerMaxHealthText;
    [SerializeField] private TextMeshProUGUI playerDamageText;
    [SerializeField] private TextMeshProUGUI playerHealingText;
    [SerializeField] private TextMeshProUGUI coinMultiplierText;
    [SerializeField] private Slider playerHealthSlider;


    private void Update()
    {
        UpdateRoundInfo();
        UpdatePlayerInfo();
    }

    private void UpdateRoundInfo()
    {
        if (waveSpawner != null)
        {
            currentRoundText.text = $"Round:{waveSpawner.CurrentRound}";
            currentRoundTierText.text = $"Tier:{waveSpawner.CurrentTier}";

            roundTimerSlider.maxValue = waveSpawner.RoundDuration;
            roundTimerSlider.value = waveSpawner.CurrentRoundTimer;
        }

        if (enemyPrefab != null)
        {
            Enemy enemy = enemyPrefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                currentBasicEnemyDamageText.text = $"D: {enemy.GetStat(EnemyStats.Damage)}";
                currentBasicEnemyHealthText.text = $"H: {enemy.GetStat(EnemyStats.Health)}";
            }
        }
    }

    private void UpdatePlayerInfo()
    {
        if (towerStats != null)
        {
            // Health
            playerHealthText.text = $"{towerStats.CurrentHealth}";
            playerMaxHealthText.text = $"/ {towerStats.MaxHealth}";
            playerHealthSlider.maxValue = towerStats.MaxHealth;
            playerHealthSlider.value = towerStats.CurrentHealth;

            // Attack
            playerDamageText.text = $"D:{towerStats.Damage}";
            playerHealingText.text = $"H:{towerStats.HealthRegen}";

            // Utility
            coinMultiplierText.text = $"C:{towerStats.CashMultiplier}x";
        }
    }
}
