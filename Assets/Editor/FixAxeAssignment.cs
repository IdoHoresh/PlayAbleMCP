using UnityEngine;
using UnityEditor;

public class FixAxeAssignment : EditorWindow
{
    [MenuItem("Tools/Fix Axe Assignment")]
    public static void Fix()
    {
        // Load the axe prefab
        GameObject axePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Equipment/_medieval_fantasy_bat_1212220047_generate.prefab");
        
        if (axePrefab == null)
        {
            Debug.LogError("Could not find axe prefab!");
            return;
        }
        
        // Load Order_Energy3
        OrderData order = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Energy3.asset");
        
        if (order == null)
        {
            Debug.LogError("Could not find Order_Energy3!");
            return;
        }
        
        // Assign the axe with correct transform settings
        order.itemPrefab3D = axePrefab;
        order.itemType = ItemType.Axe;
        order.equipmentSlot = EquipmentSlot.RightHand;
        order.equipmentScale = new Vector3(3, 3, 3);
        order.equipmentRotationOffset = new Vector3(-90, 0, 0);
        order.equipmentPositionOffset = new Vector3(0, 0, 0.05f);
        
        EditorUtility.SetDirty(order);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"✓ Fixed Order_Energy3!");
        Debug.Log($"  - itemPrefab3D: {order.itemPrefab3D != null}");
        Debug.Log($"  - itemType: {order.itemType}");
        Debug.Log($"  - equipmentSlot: {order.equipmentSlot}");
        
        // If in play mode, also try to update the runtime instance
        if (Application.isPlaying)
        {
            OrderManager orderManager = FindFirstObjectByType<OrderManager>();
            if (orderManager != null && orderManager.availableOrders != null)
            {
                for (int i = 0; i < orderManager.availableOrders.Length; i++)
                {
                    if (orderManager.availableOrders[i] != null && orderManager.availableOrders[i].name == "Order_Energy3")
                    {
                        // Force reload
                        orderManager.availableOrders[i] = order;
                        Debug.Log("✓ Updated runtime OrderManager reference");
                        
                        // Update the slot display
                        if (orderManager.orderSlots != null && i < orderManager.orderSlots.Length)
                        {
                            orderManager.orderSlots[i].SetOrder(order);
                            Debug.Log("✓ Refreshed order slot display");
                        }
                    }
                }
            }
        }
    }
}
