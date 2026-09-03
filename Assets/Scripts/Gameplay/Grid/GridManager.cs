using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width, height;
    [SerializeField] private Transform mainCamera;

    [Header("Tile Settings")]
    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

    private void Start()
    {
        GenerateGrid();
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
}
