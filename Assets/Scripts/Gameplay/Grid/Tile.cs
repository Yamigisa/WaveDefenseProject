using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highlightedColor;

    private void OnMouseEnter()
    {
        spriteRenderer.color = highlightedColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }
}
