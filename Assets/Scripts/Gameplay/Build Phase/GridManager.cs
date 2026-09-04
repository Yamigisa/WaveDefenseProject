using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width, height;
    [SerializeField] private Transform mainCamera;

    [Header("Tile Settings")]
    [SerializeField] private Tile tilePrefab;

    [Header("Enemy Destination")]
    [Min(1)] [SerializeField] private int enemyDestinationMaxHealth = 20;

    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();
    private readonly List<Tile> attackRangePreviewTiles = new();
    private Tile previewTile;
    private bool isPlacementPreviewActive;
    private Vector2Int attackRangePreviewSourceCell;
    private Vector2Int attackRangePreviewSize;

    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        GenerateGrid();
        SpawnEnemyDestination();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.name = $"Tile {x} {y}";

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        mainCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public Tile GetTilePosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetTileAtWorldPosition(Vector3 worldPosition)
    {
        Vector2 gridPosition = new Vector2(
            Mathf.FloorToInt(worldPosition.x + 0.5f),
            Mathf.FloorToInt(worldPosition.y + 0.5f));

        return GetTilePosition(gridPosition);
    }

    private void SpawnEnemyDestination()
    {
        Vector2Int centerCell = new(width / 2, height / 2);
        Tile centerTile = GetTilePosition(centerCell);
        if (centerTile == null)
            return;

        GameObject destination = new("Enemy Destination");
        destination.transform.SetParent(transform);
        destination.transform.position = centerTile.transform.position;
        destination.name = "Enemy Destination";
        EnemyDestination destinationComponent = destination.AddComponent<EnemyDestination>();
        destinationComponent.Initialize(enemyDestinationMaxHealth);
        centerTile.SetOccupied(true);
    }

    public void SetPlacementPreview(bool isActive)
    {
        isPlacementPreviewActive = isActive;

        if (!isActive)
            SetPlacementPreviewTile(null);
    }

    public void SetPlacementPreviewTile(Tile tile)
    {
        if (previewTile == tile)
            return;

        if (previewTile != null)
            previewTile.SetPlacementPreview(false);

        previewTile = isPlacementPreviewActive ? tile : null;

        if (previewTile != null)
            previewTile.SetPlacementPreview(true);
    }

    public void SetAttackRangePreview(Vector2Int sourceCell, Vector2Int areaSize)
    {
        areaSize = new Vector2Int(Mathf.Max(1, areaSize.x), Mathf.Max(1, areaSize.y));
        if (attackRangePreviewTiles.Count > 0 && sourceCell == attackRangePreviewSourceCell &&
            areaSize == attackRangePreviewSize)
        {
            return;
        }

        ClearAttackRangePreview();
        attackRangePreviewSourceCell = sourceCell;
        attackRangePreviewSize = areaSize;

        int minimumX = sourceCell.x - areaSize.x / 2;
        int minimumY = sourceCell.y - areaSize.y / 2;

        for (int x = minimumX; x < minimumX + areaSize.x; x++)
        {
            for (int y = minimumY; y < minimumY + areaSize.y; y++)
            {
                Tile tile = GetTilePosition(new Vector2(x, y));
                if (tile == null)
                    continue;

                tile.SetAttackRangePreview(true);
                attackRangePreviewTiles.Add(tile);
            }
        }
    }

    public void ClearAttackRangePreview()
    {
        foreach (Tile tile in attackRangePreviewTiles)
        {
            if (tile != null)
                tile.SetAttackRangePreview(false);
        }

        attackRangePreviewTiles.Clear();
    }
}
