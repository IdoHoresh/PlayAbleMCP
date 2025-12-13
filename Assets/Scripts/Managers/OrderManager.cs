using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OrderManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public CoinWallet coinWallet;
    public OrderSlot[] orderSlots;

    [Header("Orders")]
    public OrderData[] availableOrders;

    private List<OrderData> activeOrders = new List<OrderData>();
    private bool initialized = false;

    private void Awake()
    {
        Debug.Log("OrderManager: Awake called");
    }

    private void Start()
    {
        Debug.Log("OrderManager: Start called, scheduling initialization...");
        // Wait one frame to ensure UISetup has completed
        Invoke(nameof(InitializeOrders), 0.1f);
    }

    private void InitializeOrders()
    {
        Debug.Log("OrderManager: InitializeOrders called");

        if (initialized) return;

        // Check if everything is ready
        if (orderSlots == null || orderSlots.Length == 0)
        {
            Debug.LogError($"OrderManager: Order slots not set up! orderSlots = {(orderSlots == null ? "null" : $"array of {orderSlots.Length}")}");
            return;
        }

        if (availableOrders == null || availableOrders.Length == 0)
        {
            Debug.LogError($"OrderManager: No orders available! availableOrders = {(availableOrders == null ? "null" : $"array of {availableOrders.Length}")}");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError("OrderManager: gridManager is null!");
            return;
        }

        if (coinWallet == null)
        {
            Debug.LogError("OrderManager: coinWallet is null!");
            return;
        }

        Debug.Log($"OrderManager: All references validated. Creating {Mathf.Min(orderSlots.Length, availableOrders.Length)} orders...");

        // Create initial orders
        for (int i = 0; i < Mathf.Min(orderSlots.Length, availableOrders.Length); i++)
        {
            if (availableOrders[i] != null && orderSlots[i] != null)
            {
                activeOrders.Add(availableOrders[i]);
                orderSlots[i].SetOrder(availableOrders[i]);
                Debug.Log($"OrderManager: Set order {i}: {availableOrders[i].name}");
            }
        }

        initialized = true;
        CheckOrderAvailability();
        Debug.Log($"OrderManager: Initialized with {activeOrders.Count} orders");
    }

    public void OnItemMerged(ItemData mergedItem)
    {
        // Check if any orders can now be completed
        CheckOrderAvailability();
    }

    private void CheckOrderAvailability()
    {
        foreach (var slot in orderSlots)
        {
            if (slot.CurrentOrder != null)
            {
                bool canComplete = CanCompleteOrder(slot.CurrentOrder);
                slot.UpdateVisualState(canComplete);
            }
        }
    }

    private bool CanCompleteOrder(OrderData order)
    {
        if (order == null || order.requiredItem == null) return false;

        // Count how many of the required items are on the grid
        int count = CountItemsOnGrid(order.requiredItem);
        return count >= order.quantity;
    }

    private int CountItemsOnGrid(ItemData itemData)
    {
        int count = 0;

        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                GridCell cell = gridManager.GetCell(x, y);
                if (cell != null && cell.IsOccupied)
                {
                    MergeableItem item = cell.GetItem();
                    if (item != null && item.itemData == itemData)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    public void CompleteOrder(OrderSlot slot)
    {
        OrderData order = slot.CurrentOrder;
        if (order == null || !CanCompleteOrder(order)) return;

        // Find and remove the required items from the grid
        RemoveItemsFromGrid(order.requiredItem, order.quantity);

        // Give coin reward
        Vector3 orderWorldPos = slot.GetWorldPosition();
        coinWallet.AddCoins(order.coinReward, orderWorldPos);
        Debug.Log($"Order completed! Earned {order.coinReward} coins!");

        // Remove this order and potentially add a new one
        int slotIndex = System.Array.IndexOf(orderSlots, slot);
        if (slotIndex >= 0)
        {
            // Clear this slot
            slot.SetOrder(null);

            // You could add logic here to spawn a new random order
        }

        CheckOrderAvailability();
    }

    private void RemoveItemsFromGrid(ItemData itemData, int quantity)
    {
        int removed = 0;

        for (int x = 0; x < gridManager.gridWidth && removed < quantity; x++)
        {
            for (int y = 0; y < gridManager.gridHeight && removed < quantity; y++)
            {
                GridCell cell = gridManager.GetCell(x, y);
                if (cell != null && cell.IsOccupied)
                {
                    MergeableItem item = cell.GetItem();
                    if (item != null && item.itemData == itemData)
                    {
                        Destroy(item.gameObject);
                        gridManager.RemoveItem(new Vector2Int(x, y));
                        removed++;
                    }
                }
            }
        }
    }
}
