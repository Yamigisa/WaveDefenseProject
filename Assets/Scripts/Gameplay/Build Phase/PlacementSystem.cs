using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform placedUnitsParent;

    [Header("Unit Ghost Preview")]
    [Range(0f, 1f)][SerializeField] private float previewAlpha = 0.5f;

    private UnitSO selectedUnit;
    private GameObject ghostUnit;
    private Tile hoveredTile;
    private readonly Dictionary<SpriteRenderer, Color> ghostSpriteColors = new();

    public static PlacementSystem Instance { get; private set; }
    public bool IsPlacementMode => ghostUnit != null;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (ghostUnit == null)
            return;

        UpdateGhostPosition();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            return;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUi())
            TryPlaceSelectedUnit();
    }

    public void StartPlacement(UnitSO unit)
    {
        CancelPlacement();

        selectedUnit = unit;
        ghostUnit = Instantiate(selectedUnit.prefab);
        ConfigureGhost(ghostUnit);
        gridManager.SetPlacementPreview(true);
    }

    public void CancelPlacement()
    {
        if (gridManager != null)
            gridManager.SetPlacementPreview(false);

        if (ghostUnit != null)
            Destroy(ghostUnit);

        ghostSpriteColors.Clear();
        ghostUnit = null;
        selectedUnit = null;
        hoveredTile = null;
    }

    private void UpdateGhostPosition()
    {
        Vector3 mouseWorldPosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        hoveredTile = gridManager.GetTileAtWorldPosition(mouseWorldPosition);
        bool isOverGrid = hoveredTile != null;

        ghostUnit.SetActive(isOverGrid);

        if (!isOverGrid)
            return;

        ghostUnit.transform.position = hoveredTile.transform.position;
        SetGhostColor(!hoveredTile.IsOccupied);
    }

    private void TryPlaceSelectedUnit()
    {
        if (hoveredTile == null || hoveredTile.IsOccupied || selectedUnit == null)
            return;

        if (ResourceManager.Instance == null ||
            !ResourceManager.Instance.TrySpend(ResourceType.Gold, selectedUnit.cost))
        {
            return;
        }

        Instantiate(selectedUnit.prefab, hoveredTile.transform.position, Quaternion.identity, placedUnitsParent);
        hoveredTile.SetOccupied(true);
        CancelPlacement();
    }

    private void ConfigureGhost(GameObject preview)
    {
        ghostSpriteColors.Clear();

        foreach (Collider2D collider in preview.GetComponentsInChildren<Collider2D>())
            collider.enabled = false;

        foreach (MonoBehaviour behaviour in preview.GetComponentsInChildren<MonoBehaviour>())
            behaviour.enabled = false;

        foreach (SpriteRenderer spriteRenderer in preview.GetComponentsInChildren<SpriteRenderer>())
        {
            ghostSpriteColors.Add(spriteRenderer, spriteRenderer.color);
        }

        SetGhostColor(true);
    }

    private void SetGhostColor(bool canPlace)
    {
        foreach (KeyValuePair<SpriteRenderer, Color> sprite in ghostSpriteColors)
        {
            if (sprite.Key == null)
                continue;

            Color previewColor = canPlace ? sprite.Value : Color.red;
            previewColor.a = previewAlpha;
            sprite.Key.color = previewColor;
        }
    }

    private static bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
