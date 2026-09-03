using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour
{
    [Header("Unit Data")]
    [SerializeField] private UnitSO unitSO;
    [SerializeField] private UnitTeam team = UnitTeam.Defender;

    private int currentHealth;
    private float nextAttackTime;
    private bool isDead;
    private Coroutine goldGenerationCoroutine;

    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public UnitTeam Team => team;

    private void Awake()
    {
        currentHealth = unitSO.maxHealth;

        if (unitSO.attacksPerSecond > 0f)
            nextAttackTime = Time.time + 1f / unitSO.attacksPerSecond;
    }

    private void OnEnable()
    {
        if (unitSO != null && unitSO.goldProduced > 0 && unitSO.goldProductionInterval > 0f)
            goldGenerationCoroutine = StartCoroutine(GenerateGold());
    }

    private void OnDisable()
    {
        SetAttackRangeVisible(false);

        if (goldGenerationCoroutine != null)
        {
            StopCoroutine(goldGenerationCoroutine);
            goldGenerationCoroutine = null;
        }
    }

    private void Update()
    {
        TryAttack();
    }

    private void OnMouseEnter()
    {
        SetAttackRangeVisible(true);
    }

    private void OnMouseExit()
    {
        SetAttackRangeVisible(false);
    }

    public void SetAttackRangeVisible(bool isVisible)
    {
        if (GridManager.Instance == null)
            return;

        if (!isVisible || !HasAttackArea())
        {
            GridManager.Instance.ClearAttackRangePreview();
            return;
        }

        GridManager.Instance.SetAttackRangePreview(
            WorldToGridCell(transform.position),
            GetAttackAreaSize());
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private void TryAttack()
    {
        if (isDead || unitSO.attack <= 0 || unitSO.attacksPerSecond <= 0f ||
            Time.time < nextAttackTime)
        {
            return;
        }

        List<UnitObject> targets = FindTargetsInRange();
        if (targets.Count == 0)
            return;

        foreach (UnitObject target in targets)
            LaunchProjectile(target);

        nextAttackTime = Time.time + 1f / unitSO.attacksPerSecond;
    }

    private void LaunchProjectile(UnitObject target)
    {
        Projectile projectile;

        if (unitSO.projectilePrefab != null)
        {
            projectile = Instantiate(unitSO.projectilePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            projectile = CreateFallbackProjectile();
        }

        projectile.Initialize(target, team, unitSO.attack, unitSO.projectileSpeed, unitSO.projectileLifetime);
    }

    private Projectile CreateFallbackProjectile()
    {
        GameObject projectileObject = new GameObject($"{unitSO.unitName} Projectile");
        projectileObject.transform.position = transform.position;
        projectileObject.transform.localScale = Vector3.one * 0.25f;

        SpriteRenderer spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();
        SpriteRenderer unitSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (unitSpriteRenderer != null)
        {
            spriteRenderer.sprite = unitSpriteRenderer.sprite;
            spriteRenderer.sharedMaterial = unitSpriteRenderer.sharedMaterial;
            spriteRenderer.sortingLayerID = unitSpriteRenderer.sortingLayerID;
            spriteRenderer.sortingOrder = unitSpriteRenderer.sortingOrder + 10;
        }
        else
        {
            spriteRenderer.sprite = unitSO.sprite;
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 10;
        }

        spriteRenderer.color = Color.yellow;

        return projectileObject.AddComponent<Projectile>();
    }

    private List<UnitObject> FindTargetsInRange()
    {
        List<UnitObject> targets = new();

        foreach (UnitObject possibleTarget in FindObjectsByType<UnitObject>(FindObjectsSortMode.None))
        {
            if (possibleTarget == null || possibleTarget == this || possibleTarget.IsDead ||
                possibleTarget.Team == team || !IsTargetInGridRange(possibleTarget))
            {
                continue;
            }

            targets.Add(possibleTarget);
        }

        targets.Sort((first, second) =>
        {
            float firstDistanceSquared = ((Vector2)(first.transform.position - transform.position)).sqrMagnitude;
            float secondDistanceSquared = ((Vector2)(second.transform.position - transform.position)).sqrMagnitude;
            return firstDistanceSquared.CompareTo(secondDistanceSquared);
        });

        int targetLimit = Mathf.Max(1, unitSO.maxTargets);
        if (targets.Count > targetLimit)
            targets.RemoveRange(targetLimit, targets.Count - targetLimit);

        return targets;
    }

    private bool IsTargetInGridRange(UnitObject target)
    {
        Vector2Int attackAreaSize = GetAttackAreaSize();
        Vector2Int sourceCell = WorldToGridCell(transform.position);
        Vector2Int targetCell = WorldToGridCell(target.transform.position);
        int horizontalOffset = targetCell.x - sourceCell.x;
        int verticalOffset = targetCell.y - sourceCell.y;

        return IsOffsetInArea(horizontalOffset, attackAreaSize.x) &&
            IsOffsetInArea(verticalOffset, attackAreaSize.y);
    }

    private bool HasAttackArea()
    {
        return unitSO != null && unitSO.attack > 0 &&
            unitSO.attackAreaWidth > 0 && unitSO.attackAreaHeight > 0;
    }

    private Vector2Int GetAttackAreaSize()
    {
        return new Vector2Int(
            Mathf.Max(1, unitSO.attackAreaWidth),
            Mathf.Max(1, unitSO.attackAreaHeight));
    }

    private static bool IsOffsetInArea(int offset, int areaSize)
    {
        int minimumOffset = -(areaSize / 2);
        int maximumOffset = minimumOffset + areaSize - 1;
        return offset >= minimumOffset && offset <= maximumOffset;
    }

    private static Vector2Int WorldToGridCell(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x + 0.5f),
            Mathf.FloorToInt(worldPosition.y + 0.5f));
    }

    private IEnumerator GenerateGold()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(unitSO.goldProductionInterval);

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.Add(ResourceType.Gold, unitSO.goldProduced);
        }
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

}

public enum UnitTeam
{
    Defender,
    Enemy
}
