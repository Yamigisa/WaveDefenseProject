using UnityEngine;

[RequireComponent(typeof(Health), typeof(Attack))]
[RequireComponent(typeof(Movement))]
public class EnemyObject : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private Vector2 initialMoveDirection = Vector2.left;

    private Health blockingUnit;
    private Attack attack;
    private Movement movement;
    private int destinationDamage;

    private void Awake()
    {
        Collider2D bodyCollider = GetComponent<Collider2D>() ?? gameObject.AddComponent<CircleCollider2D>();
        bodyCollider.isTrigger = false;

        Health health = GetComponent<Health>() ?? gameObject.AddComponent<Health>();
        attack = GetComponent<Attack>() ?? gameObject.AddComponent<Attack>();
        movement = GetComponent<Movement>() ?? gameObject.AddComponent<Movement>();

        if (enemySO == null)
        {
            Debug.LogError($"{name} is missing its EnemySO.", this);
            enabled = false;
            return;
        }

        health.Initialize(enemySO.maxHealth);
        attack.Initialize(health, enemySO.attack, new Vector2Int(enemySO.attackAreaWidth, enemySO.attackAreaHeight),
            enemySO.maxTargets, enemySO.attacksPerSecond, enemySO.projectilePrefab, enemySO.projectileSpeed, enemySO.projectileLifetime,
            enemySO.attackType);
        movement.Initialize(enemySO.moveSpeed, initialMoveDirection);
        destinationDamage = Mathf.Max(1, enemySO.destinationDamage);
    }

    private void Update()
    {
        if (EnemyDestination.Instance != null)
            movement.SetDestination(EnemyDestination.Instance.transform);

        if (blockingUnit != null && !blockingUnit.IsDead)
            return;

        blockingUnit = null;
        attack.SetPriorityTarget(null);
        movement.SetBlocked(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (blockingUnit != null)
            return;

        Health unit = collision.collider.GetComponentInParent<Health>();
        if (unit == null || unit.GetComponent<UnitObject>() == null)
            return;

        blockingUnit = unit;
        attack.SetPriorityTarget(unit);
        movement.SetBlocked(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyDestination destination = other.GetComponentInParent<EnemyDestination>();
        if (destination == null)
            return;

        destination.ReceiveEnemy(destinationDamage);
        Destroy(gameObject);
    }
}
