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

    private void Start()
    {
        InitializeOrders();
    }

    private void InitializeOrders()
    {
        // Create initial orders
        for (int i = 0; i < Mathf.Min(orderSlots.Length, availableOrders.Length); i++)
        {
            if (availableOrders[i] != null)
            {
                activeOrders.Add(availableOrders[i]);
                orderSlots[i].SetOrder(availableOrders[i]);
            }
        }

        CheckOrderAvailability();
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

        // Give coin reward with animation
        Vector3 orderWorldPos = slot.GetWorldPosition();
        coinWallet.AddCoins(order.coinReward, orderWorldPos);

        // Remove this order and potentially add a new one
        int slotIndex = System.Array.IndexOf(orderSlots, slot);
        if (slotIndex >= 0)
        {
            // Clear this slot
            slot.SetOrder(null);

            // You could add logic here to spawn a new random order
            Debug.Log($"Order completed! Earned {order.coinReward} coins!");
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
