using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("References")]
    public MergeManager mergeManager;
    public GridManager gridManager;

    [Header("Test Items")]
    public ItemData[] testItems;

    [Header("Initial Setup")]
    public bool spawnTestItems = true;

    private void Start()
    {
        if (spawnTestItems && testItems.Length > 0)
        {
            SpawnInitialItems();
        }
    }

    private void SpawnInitialItems()
    {
        // Spawn a few test items on the grid
        // Example: Place wood level 1 at different positions
        
        if (testItems.Length > 0 && testItems[0] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[0], new Vector2Int(0, 0));
            mergeManager.CreateItemAtPosition(testItems[0], new Vector2Int(1, 0));
        }
        
        if (testItems.Length > 1 && testItems[1] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[1], new Vector2Int(2, 0));
        }

        Debug.Log("Initial test items spawned");
    }
}