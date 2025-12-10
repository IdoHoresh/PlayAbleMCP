using UnityEngine;

public class MergeManager : MonoBehaviour
{
    private GridManager gridManager;
    private OrderManager orderManager;

    [Header("Prefabs")]
    public GameObject mergeableItemPrefab;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        orderManager = FindFirstObjectByType<OrderManager>();
    }

    public void MergeItems(MergeableItem item1, MergeableItem item2, Vector2Int mergePosition)
    {
        if (!item1.CanMergeWith(item2))
        {
            Debug.LogWarning("Cannot merge these items!");
            return;
        }

        ItemData resultData = item1.GetMergeResult();
        
        if (resultData == null)
        {
            Debug.LogWarning($"No merge result defined for {item1.itemData.itemName}!");
            return;
        }

        // Destroy both items
        Destroy(item1.gameObject);
        Destroy(item2.gameObject);

        // Clear the grid cell
        gridManager.RemoveItem(mergePosition);

        // Create new merged item
        CreateItemAtPosition(resultData, mergePosition);

        // Notify order manager
        if (orderManager != null)
        {
            orderManager.OnItemMerged(resultData);
        }

        Debug.Log($"Merged into {resultData.itemName} at {mergePosition}");
    }

    public MergeableItem CreateItemAtPosition(ItemData itemData, Vector2Int gridPosition)
    {
        if (mergeableItemPrefab == null)
        {
            Debug.LogError("MergeableItem prefab not assigned!");
            return null;
        }

        Vector2 worldPos = gridManager.GridToWorldPosition(gridPosition.x, gridPosition.y);
        GameObject itemObj = Instantiate(mergeableItemPrefab, worldPos, Quaternion.identity);
        
        MergeableItem item = itemObj.GetComponent<MergeableItem>();
        if (item != null)
        {
            item.Initialize(itemData);
            gridManager.PlaceItem(item, gridPosition);

            // Play spawn effect
            ItemVisualEffects effects = item.GetComponent<ItemVisualEffects>();
            if (effects != null)
            {
                effects.PlaySpawnEffect();
            }
        }

        return item;
    }
}