using UnityEngine;

public class Movement : MonoBehaviour
{
    private float moveSpeed;
    private Vector2 moveDirection;
    private Rigidbody2D body;
    private Transform destination;
    private bool isBlocked;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void Initialize(float newMoveSpeed, Vector2 initialDirection)
    {
        moveSpeed = Mathf.Max(0f, newMoveSpeed);
        SetDirection(initialDirection);
    }

    public void SetDirection(Vector2 newDirection)
    {
        moveDirection = newDirection.sqrMagnitude > 0.0001f ? newDirection.normalized : Vector2.zero;
    }

    public void SetDestination(Transform newDestination)
    {
        destination = newDestination;
    }

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
    }

    private void FixedUpdate()
    {
        if (body == null || isBlocked || moveSpeed <= 0f)
            return;

        if (destination != null)
            SetDirection((Vector2)(destination.position - transform.position));

        if (moveDirection == Vector2.zero)
            return;

        body.MovePosition(body.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
