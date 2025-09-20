using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    // ----- ATTACK -----
    Damage,
    AttackSpeed,
    CritChance,
    CritMultiplier,
    Range,

    // ----- DEFENSE -----
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
    public float baseValue;
    public float value;

    public Stat() { }

    public Stat(StatType type, float baseValue)
    {
        this.type = type;
        this.baseValue = baseValue;
        this.value = baseValue;
    }
}

public class TowerStats : MonoBehaviour
{
    [Header("Attack Stats")]
    public List<Stat> attackStats = new List<Stat>()
    {
        new Stat(StatType.Damage, 3f),
        new Stat(StatType.AttackSpeed, 1f),
        new Stat(StatType.CritChance, 0f),
        new Stat(StatType.CritMultiplier, 1f),
        new Stat(StatType.Range, 5f)
    };

    [Header("Defense Stats")]
    [SerializeField] private float currentHealth;
    public List<Stat> defenceStats = new List<Stat>()
    {
        new Stat(StatType.MaxHealth, 5f),
        new Stat(StatType.HealthRegen, 0.09f),
        new Stat(StatType.DamageReduction, 0f),
        new Stat(StatType.Armor, 0f)
    };

    [Header("Utility Stats")]
    public List<Stat> utilityStats = new List<Stat>()
    {
        new Stat(StatType.CashMultiplier, 1f),
        new Stat(StatType.CashPerWave, 0f),
        new Stat(StatType.CoinsPerKill, 0f),
        new Stat(StatType.CoinsPerWave, 0f)
    };

    // --- UI-friendly properties ---
    public float CurrentHealth => currentHealth;
    public float MaxHealth => GetStat(StatType.MaxHealth);
    public float Damage => GetStat(StatType.Damage);
    public float HealthRegen => GetStat(StatType.HealthRegen);
    public float CashMultiplier => GetStat(StatType.CashMultiplier);


    [Header("Targeting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform partToRotate;
    public string enemyTag = "Enemy";

    private Transform target;
    private float fireCountdown = 0f;

    public GameObject mainMenu;
    public GameObject mainHud;

    void Start()
    {
        ResetStats(); // apply permanent upgrades
        currentHealth = MaxHealth;
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    void Update()
    {
        if (target != null)
        {
            RotateTowardTarget();
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / GetStat(StatType.AttackSpeed);
            }
        }

        fireCountdown -= Time.deltaTime;

        // Health regeneration
        if (currentHealth < MaxHealth)
        {
            currentHealth += HealthRegen * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= GetStat(StatType.Range))
            target = nearestEnemy.transform;
        else
            target = null;
    }

    void RotateTowardTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot()
    {
        if (bulletPrefab == null || target == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Seek(target);
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

    public void ResetStats()
    {
        List<Stat>[] statLists = { attackStats, defenceStats, utilityStats };

        foreach (var list in statLists)
        {
            foreach (var stat in list)
            {
                // Apply permanent upgrades on top of base value
                float upgradedValue = PermanentUpgradesManager.Instance.ApplyUpgrades(stat.type, stat.baseValue);

                // DO NOT overwrite baseValue
                stat.value = upgradedValue;

                // If MaxHealth, update currentHealth
                if (stat.type == StatType.MaxHealth)
                    currentHealth = upgradedValue;
            }
        }
    }

    public void ApplyPermanentUpgrade(StatType type, float newValue)
    {
        List<Stat>[] statLists = { attackStats, defenceStats, utilityStats };

        foreach (var list in statLists)
        {
            foreach (var stat in list)
            {
                if (stat.type == type)
                    stat.value = newValue;
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
    }
    public void TakeDamage(float amount)
    {
        float damageReduction = GetStat(StatType.DamageReduction);
        float damageTaken = amount * (1f - damageReduction);

        float armor = GetStat(StatType.Armor);
        damageTaken -= armor;
        damageTaken = Mathf.Max(0f, damageTaken);

        currentHealth -= damageTaken;
        if (currentHealth <= 0f)
            Die();
    }
    void Die()
    {
        Destroy(gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");


        mainHud.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        
    
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Get the enemy's damage from its stats
                float damage = enemy.GetStat(EnemyStats.Damage);
                Debug.Log("Tower hit by enemy for " + damage + " damage.");
                TakeDamage(damage);
                Destroy(collision.gameObject);
            }
        }
    }
}



