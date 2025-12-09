using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool IsOccupied => currentItem != null;
    
    private MergeableItem currentItem;
    private SpriteRenderer spriteRenderer;
    
    [Header("Visual Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    public Color highlightColor = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

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