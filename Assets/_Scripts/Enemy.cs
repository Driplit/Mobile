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
    public float baseHealth = 10f;
    public float baseDamage = 2f;
    public float baseSpeed = 5f;

    [Header("Scaling Settings")]
    public float roundExponentBase = 1.05f;   // exponential difficulty scaling
    public float linearHealthGrowth = 0.2f;
    public float linearDamageGrowth = 0.2f;

    [Header("Rewards")]
    public int moneyReward = 1;  // Soft currency
    public int coinsReward = 0;  // Hard currency

    [Header("References")]
    private Transform player;
    private Wallet wallet;

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
    }

    private void Update()
    {
        if (player == null) return;

        // Move toward player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // stay grounded

        if (direction.magnitude > 0.1f)
        {
            Vector3 move = direction.normalized * currentSpeed * Time.deltaTime;
            transform.position += move;

            // Smooth rotate toward player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    public void CalculateStats(int roundNumber)
    {
        int typeMultiplier = enemyTypeMultiplier[enemyType];

        float scaledHealth = (baseHealth * typeMultiplier) + (linearHealthGrowth * roundNumber);
        float scaledDamage = (baseDamage * typeMultiplier) + (linearDamageGrowth * roundNumber);

        currentHealth = scaledHealth * Mathf.Pow(roundExponentBase, roundNumber);
        currentDamage = scaledDamage * Mathf.Pow(roundExponentBase, roundNumber);
        currentSpeed = baseSpeed; // speed could also scale if you want
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        if (wallet != null)
        {
            wallet.AddMoney(moneyReward);
            wallet.AddCoins(coinsReward);
        }
        else
        {
            Debug.LogWarning("Wallet reference missing!");
        }

        Destroy(gameObject);
    }

    // For UI or other scripts to fetch a stat
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
}
