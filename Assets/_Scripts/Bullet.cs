using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public float lifetime = 5f; // Seconds before self-destruct
    private float damage;


    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Start()
    {
        //get the player's current damage
        TowerStats towerStats = GameObject.FindWithTag("Player").GetComponent<TowerStats>();
        damage = Mathf.RoundToInt(towerStats.GetStat(StatType.Damage));

        // Destroy bullet if it exists too long
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Direction to target
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // If close enough to hit this frame
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // Move toward target
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Debug.Log("Bullet hit: " + target.name);

        // Example damage logic
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject); // Destroy bullet on hit
    }
}