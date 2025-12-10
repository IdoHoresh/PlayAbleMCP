using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool IsOccupied => currentItem != null;
    
    private MergeableItem currentItem;
    private SpriteRenderer spriteRenderer;
    
    [Header("Visual Settings")]
    public Color normalColor = new Color(0.93f, 0.76f, 0.6f, 0.8f); // Peachy tan color
    public Color highlightColor = new Color(1f, 0.9f, 0.7f, 0.9f); // Lighter peach for valid placement
    public Color invalidColor = new Color(1f, 0.6f, 0.5f, 0.8f); // Reddish peach for invalid

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    public void SetItem(MergeableItem item)
    {
        currentItem = item;
        if (item != null)
        {
            item.SetGridCell(this);
        }
    }

    public MergeableItem GetItem()
    {
        return currentItem;
    }

    public void ClearItem()
    {
        currentItem = null;
    }

    public void Highlight(bool valid)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = valid ? highlightColor : invalidColor;
        }
    }

    public void ResetVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }
}