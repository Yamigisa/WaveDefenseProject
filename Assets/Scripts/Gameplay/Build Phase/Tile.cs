using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Placement Preview Colors")]
    [SerializeField] private Color validPlacementColor = new(0.35f, 0.9f, 0.35f, 1f);
    [SerializeField] private Color invalidPlacementColor = new(0.9f, 0.35f, 0.35f, 1f);

    [Header("Attack Range Preview")]
    [SerializeField] private Color attackRangeColor = new(0.3f, 0.65f, 1f, 1f);

    private bool isOccupied = false;
    private bool isShowingPlacementPreview;
    private bool isShowingAttackRangePreview;
    private Color defaultColor;

    public bool IsOccupied => isOccupied;

    private void Awake()
    {
        defaultColor = spriteRenderer.color;
    }

    public void SetPlacementPreview(bool isActive)
    {
        isShowingPlacementPreview = isActive;
        RefreshColor();
    }

    public void SetAttackRangePreview(bool isActive)
    {
        isShowingAttackRangePreview = isActive;
        RefreshColor();
    }

    public void SetOccupied(bool value)
    {
        isOccupied = value;
        RefreshColor();
    }

    private void RefreshColor()
    {
        if (spriteRenderer == null)
            return;

        if (isShowingPlacementPreview)
        {
            spriteRenderer.color = isOccupied ? invalidPlacementColor : validPlacementColor;
            return;
        }

        spriteRenderer.color = isShowingAttackRangePreview ? attackRangeColor : defaultColor;
    }
}
