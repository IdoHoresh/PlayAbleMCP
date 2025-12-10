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
        // Spawn gems and energy items on the grid

        // Gem Level 1 - two items for merging
        if (testItems.Length > 0 && testItems[0] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[0], new Vector2Int(0, 0));
            mergeManager.CreateItemAtPosition(testItems[0], new Vector2Int(1, 0));
        }

        // Gem Level 2
        if (testItems.Length > 1 && testItems[1] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[1], new Vector2Int(2, 0));
        }

        // Energy Level 1 - two items for merging
        if (testItems.Length > 2 && testItems[2] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[2], new Vector2Int(0, 1));
            mergeManager.CreateItemAtPosition(testItems[2], new Vector2Int(1, 1));
        }

        // Energy Level 2
        if (testItems.Length > 3 && testItems[3] != null)
        {
            mergeManager.CreateItemAtPosition(testItems[3], new Vector2Int(2, 1));
        }

        Debug.Log("Initial items spawned: Gems and Energy");
    }
}