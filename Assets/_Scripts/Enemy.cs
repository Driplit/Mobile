using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Basic,
    Fast,
    Tank,
    Boss
}

public enum EnemyStats
{
    Health,
    Speed,
    Damage
}

[System.Serializable]
public class EnemyStat
{
    public EnemyStats type;
    public float value;
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("Current Stats (Runtime)")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentDamage;
    [SerializeField] private float currentSpeed;

    [Header("Base Stats (Editable)")]
    public float baseHealth = 1f;
    public float baseDamage = 2f;
    public float baseSpeed = 5f;

    [Header("Scaling Settings")]
    public float roundExponentBase = 1.05f;
    public float linearHealthGrowth = 0.2f;
    public float linearDamageGrowth = 0.2f;

    [Header("Rewards")]
    public int cashReward = 1;
    public int coinsReward = 0;

    [Header("References")]
    private Transform player;
    private Wallet wallet;
    private TierTracker tierTracker;

    // multipliers by enemy type
    private Dictionary<EnemyType, int> enemyTypeMultiplier = new Dictionary<EnemyType, int>()
    {
        { EnemyType.Basic, 1 },
        { EnemyType.Fast, 1 },
        { EnemyType.Tank, 2 },
        { EnemyType.Boss, 5 }
    };

    private void Awake()
    {
        wallet = FindAnyObjectByType<Wallet>();
        player = GameObject.FindWithTag("Player")?.transform;
        tierTracker = FindAnyObjectByType<TierTracker>();
    }

    private void Update()
    {

        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Vector3 move = direction.normalized * currentSpeed * Time.deltaTime;
            transform.position += move;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        
    }

    /// <summary>
    /// Calculates stats based on round and tier.
    /// </summary>
    public void CalculateStats(int roundNumber)
    {
        int typeMultiplier = enemyTypeMultiplier[enemyType];

        // Base round scaling
        float scaledHealth = (baseHealth * typeMultiplier) + (linearHealthGrowth * roundNumber);
        float scaledDamage = (baseDamage * typeMultiplier) + (linearDamageGrowth * roundNumber);

        scaledHealth *= Mathf.Pow(roundExponentBase, roundNumber);
        scaledDamage *= Mathf.Pow(roundExponentBase, roundNumber);

        // Grab current tier from TierTracker (default 1 if null)
        int tier = tierTracker != null ? tierTracker.CurrentTier : 1;

        // Apply tier multipliers
        float tierMultiplier = 1f;
        if (tier == 2) tierMultiplier = 2.0f;           // +10%
        if (tier == 3) tierMultiplier = 3.0f;   // +26.5%

        currentHealth = scaledHealth * tierMultiplier;
        currentDamage = scaledDamage * tierMultiplier;
        currentSpeed = baseSpeed; // optionally scale speed too
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (wallet != null)
        {
            wallet.AddCash(cashReward);
            wallet.AddCoins(coinsReward);
        }

        Destroy(gameObject);
    }

    public float GetStat(EnemyStats type)
    {
        return type switch
        {
            EnemyStats.Health => currentHealth,
            EnemyStats.Damage => currentDamage,
            EnemyStats.Speed => currentSpeed,
            _ => 0f
        };
    }

    /// <summary>
    /// Returns preview stats for a round and optional tier (used in UI).
    /// </summary>
    public (float health, float damage) GetPreviewStats(int roundNumber, int tier = 1)
    {
        int typeMultiplier = enemyTypeMultiplier[enemyType];

        float scaledHealth = (baseHealth * typeMultiplier) + (linearHealthGrowth * roundNumber);
        float scaledDamage = (baseDamage * typeMultiplier) + (linearDamageGrowth * roundNumber);

        scaledHealth *= Mathf.Pow(roundExponentBase, roundNumber);
        scaledDamage *= Mathf.Pow(roundExponentBase, roundNumber);

        
        float tierMultiplier = 1f;
        if (tier == 2) tierMultiplier = 1.10f;
        if (tier == 3) tierMultiplier = 1.10f * 1.15f;

        return (scaledHealth * tierMultiplier, scaledDamage * tierMultiplier);
    }



}
