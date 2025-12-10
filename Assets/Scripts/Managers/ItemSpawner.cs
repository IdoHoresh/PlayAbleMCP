using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public MergeManager mergeManager;
    public GridManager gridManager;

    [Header("Items to Spawn")]
    public ItemData[] spawnableItems;

    [Header("Spawn Settings")]
    public bool spawnOnStart = true;
    public int itemsToSpawnOnStart = 3;

    private void Start()
    {
        if (spawnOnStart && spawnableItems != null && spawnableItems.Length > 0)
        {
            SpawnRandomItems(itemsToSpawnOnStart);
        }
    }

    private void Update()
    {
        // Press Space to spawn a random item
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnRandomItem();
        }

        // Press 1-9 to spawn specific items
        for (int i = 0; i < Mathf.Min(9, spawnableItems.Length); i++)
        {
            if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame && i == 0 ||
                Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame && i == 1 ||
                Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame && i == 2 ||
                Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame && i == 3 ||
                Keyboard.current != null && Keyboard.current.digit5Key.wasPressedThisFrame && i == 4 ||
                Keyboard.current != null && Keyboard.current.digit6Key.wasPressedThisFrame && i == 5 ||
                Keyboard.current != null && Keyboard.current.digit7Key.wasPressedThisFrame && i == 6 ||
                Keyboard.current != null && Keyboard.current.digit8Key.wasPressedThisFrame && i == 7 ||
                Keyboard.current != null && Keyboard.current.digit9Key.wasPressedThisFrame && i == 8)
            {
                SpawnSpecificItem(i);
            }
        }
    }

    private void SpawnRandomItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem()
    {
        if (spawnableItems == null || spawnableItems.Length == 0)
        {
            Debug.LogWarning("No spawnable items configured!");
            return;
        }

        Vector2Int? emptyCell = FindRandomEmptyCell();
        if (emptyCell.HasValue)
        {
            ItemData randomItem = spawnableItems[Random.Range(0, spawnableItems.Length)];
            mergeManager.CreateItemAtPosition(randomItem, emptyCell.Value);
            Debug.Log($"Spawned {randomItem.itemName} at {emptyCell.Value}");
        }
        else
        {
            Debug.LogWarning("No empty cells available!");
        }
    }

    private void SpawnSpecificItem(int index)
    {
        if (index < 0 || index >= spawnableItems.Length)
        {
            Debug.LogWarning($"Item index {index} out of range!");
            return;
        }

        Vector2Int? emptyCell = FindRandomEmptyCell();
        if (emptyCell.HasValue)
        {
            ItemData item = spawnableItems[index];
            mergeManager.CreateItemAtPosition(item, emptyCell.Value);
            Debug.Log($"Spawned {item.itemName} at {emptyCell.Value} (Press {index + 1})");
        }
        else
        {
            Debug.LogWarning("No empty cells available!");
        }
    }

    private Vector2Int? FindRandomEmptyCell()
    {
        // Try to find an empty cell randomly
        int maxAttempts = 100;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int x = Random.Range(0, gridManager.gridWidth);
            int y = Random.Range(0, gridManager.gridHeight);
            Vector2Int position = new Vector2Int(x, y);

            if (!gridManager.IsCellOccupied(position))
            {
                return position;
            }
        }

        return null;
    }
}
