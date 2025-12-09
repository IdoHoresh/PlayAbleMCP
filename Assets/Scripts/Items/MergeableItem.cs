using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MergeableItem : MonoBehaviour
{
    public ItemData itemData;
    
    private SpriteRenderer spriteRenderer;
    private GridCell currentCell;
    private Vector2Int gridPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void Initialize(ItemData data)
    {
        itemData = data;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (itemData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.sprite;
            spriteRenderer.color = itemData.color;
        }
    }

    public void SetGridCell(GridCell cell)
    {
        currentCell = cell;
        if (cell != null)
        {
            gridPosition = cell.gridPosition;
        }
    }

    public GridCell GetGridCell()
    {
        return currentCell;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public bool CanMergeWith(MergeableItem other)
    {
        if (other == null || itemData == null) return false;
        return itemData.CanMergeWith(other.itemData);
    }

    public ItemData GetMergeResult()
    {
        if (itemData != null)
        {
            return itemData.mergesInto;
        }
        return null;
    }
}