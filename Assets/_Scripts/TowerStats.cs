using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    // ----- ATTACK -----
    Damage,
    AttackSpeed,
    CritChance,
    CritMultiplier,
    Range,

    // ----- DEFENCE -----
    MaxHealth,
    HealthRegen,
    DamageReduction,
    Armor,

    // ----- UTILITY -----
    CashMultiplier,
    CashPerWave,
    CoinsPerKill,
    CoinsPerWave
}

[System.Serializable]
public class Stat
{
    public StatType type;
    public float value;
}

public class TowerStats : MonoBehaviour
{
    [Header("Attack Stats")]
    public List<Stat> attackStats = new List<Stat>()
    {
        new Stat() { type = StatType.Damage,         value = 3f },
        new Stat() { type = StatType.AttackSpeed,    value = 1f },
        new Stat() { type = StatType.CritChance,     value = 0f },
        new Stat() { type = StatType.CritMultiplier, value = 1f },
        new Stat() { type = StatType.Range,          value = 5f }
    };

    [Header("Defence Stats")]
    [SerializeField] private float currentHealth;
    public List<Stat> defenceStats = new List<Stat>()
    {
        new Stat() { type = StatType.MaxHealth,         value = 5f },
        new Stat() { type = StatType.HealthRegen,       value = 0.09f },
        new Stat() { type = StatType.DamageReduction,   value = 0f },
        new Stat() { type = StatType.Armor,             value = 0f }
    };

    [Header("Utility Stats")]
    public List<Stat> utilityStats = new List<Stat>()
    {
        new Stat() { type = StatType.CashMultiplier, value = 1f },
        new Stat() { type = StatType.CashPerWave,    value = 0f },
        new Stat() { type = StatType.CoinsPerKill,   value = 0f },
        new Stat() { type = StatType.CoinsPerWave,   value = 0f }
    };

    [Header("Targeting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform target;
    public Transform partToRotate;
    public string enemyTag = "Enemy";

    private float fireCountdown = 0f;

    // --- UI-friendly properties ---
    public float CurrentHealth => currentHealth;
    public float MaxHealth => GetStat(StatType.MaxHealth);
    public float Damage => GetStat(StatType.Damage);
    public float HealthRegen => GetStat(StatType.HealthRegen);
    public float CashMultiplier => GetStat(StatType.CashMultiplier);

    void Start()
    {
        currentHealth = MaxHealth; // Start full health
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= GetStat(StatType.Range))
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Rotate tower to face target
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / GetStat(StatType.AttackSpeed);
            }
        }

        fireCountdown -= Time.deltaTime;

        // Health regen
        if (currentHealth < MaxHealth)
        {
            currentHealth += GetStat(StatType.HealthRegen) * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        }
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    public float GetStat(StatType type)
    {
        foreach (var stat in attackStats)
            if (stat.type == type) return stat.value;

        foreach (var stat in defenceStats)
            if (stat.type == type) return stat.value;

        foreach (var stat in utilityStats)
            if (stat.type == type) return stat.value;

        return 0f;
    }
}
