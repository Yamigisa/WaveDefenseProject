using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float hitDistance = 0.1f;

    private Health target;
    private int damage;
    private float moveSpeed;
    private bool hasHit;
    private Vector3 travelDirection;

    public void Initialize(Health newTarget, int newDamage,
        float newMoveSpeed, float lifetime)
    {
        target = newTarget;
        damage = newDamage;
        moveSpeed = newMoveSpeed;

        travelDirection = target != null
            ? (target.transform.position - transform.position).normalized
            : transform.right;

        if (travelDirection.sqrMagnitude < 0.0001f)
            travelDirection = Vector3.right;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hasHit)
        {
            Destroy(gameObject);
            return;
        }

        if (target != null && !target.IsDead)
        {
            Vector3 directionToTarget = target.transform.position - transform.position;

            if (directionToTarget.sqrMagnitude > 0.0001f)
                travelDirection = directionToTarget.normalized;

            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                moveSpeed * Time.deltaTime);
            transform.right = travelDirection;

            if (Vector2.Distance(transform.position, target.transform.position) <= hitDistance)
            {
                HitTarget(target);
                return;
            }

            return;
        }

        transform.position += travelDirection * moveSpeed * Time.deltaTime;
        transform.right = travelDirection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health hitUnit = other.GetComponentInParent<Health>();

        if (!hasHit && hitUnit != null && hitUnit == target)
            HitTarget(hitUnit);
    }

    private void HitTarget(Health hitUnit)
    {
        hasHit = true;
        hitUnit.TakeDamage(damage);
        Destroy(gameObject);
    }
}
