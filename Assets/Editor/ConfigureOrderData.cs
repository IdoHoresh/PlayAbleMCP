using UnityEngine;
using UnityEditor;
using System.IO;

public class ConfigureOrderData : EditorWindow
{
    [MenuItem("Tools/Configure Order Data")]
    public static void ConfigureOrders()
    {
        // Load ItemData assets
        ItemData energy3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Energy_Level3.asset");
        ItemData gem3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Gem_Level3.asset");
        ItemData pinata3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Pinata_Level3.asset");

        // Check if Order_Gem3 exists, if not create it
        OrderData orderGem3 = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Gem3.asset");
        if (orderGem3 == null)
        {
            // Create new Order_Gem3 asset
            orderGem3 = ScriptableObject.CreateInstance<OrderData>();
            AssetDatabase.CreateAsset(orderGem3, "Assets/Data/Orders/Order_Gem3.asset");
            Debug.Log("Created new Order_Gem3.asset");
        }

        // Load OrderData assets
        OrderData orderEnergy3 = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Energy3.asset");
        OrderData orderPinata3 = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Pinata3.asset");

        bool changed = false;

        // Configure Order_Energy3 (requires 1x Energy Level 3)
        if (orderEnergy3 != null && energy3 != null)
        {
            orderEnergy3.requiredItem = energy3;
            orderEnergy3.quantity = 1;
            orderEnergy3.coinReward = 100;
            orderEnergy3.itemType = ItemType.Energy;
            orderEnergy3.equipmentSlot = EquipmentSlot.None;
            EditorUtility.SetDirty(orderEnergy3);
            Debug.Log("Configured Order_Energy3: 1x Energy Level 3, Reward: 100 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Energy3 or Energy_Level3. orderEnergy3={orderEnergy3}, energy3={energy3}");
        }

        // Configure Order_Gem3 (requires 1x Gem Level 3)
        if (orderGem3 != null && gem3 != null)
        {
            orderGem3.requiredItem = gem3;
            orderGem3.quantity = 1;
            orderGem3.coinReward = 120;
            orderGem3.itemType = ItemType.Gem;
            orderGem3.equipmentSlot = EquipmentSlot.None;
            EditorUtility.SetDirty(orderGem3);
            Debug.Log("Configured Order_Gem3: 1x Gem Level 3, Reward: 120 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Gem3 or Gem_Level3. orderGem3={orderGem3}, gem3={gem3}");
        }

        // Configure Order_Pinata3 (requires 1x Pinata Level 3)
        if (orderPinata3 != null && pinata3 != null)
        {
            orderPinata3.requiredItem = pinata3;
            orderPinata3.quantity = 1;
            orderPinata3.coinReward = 150;
            orderPinata3.itemType = ItemType.Food;
            orderPinata3.equipmentSlot = EquipmentSlot.None;
            EditorUtility.SetDirty(orderPinata3);
            Debug.Log("Configured Order_Pinata3: 1x Pinata Level 3, Reward: 150 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Pinata3 or Pinata_Level3. orderPinata3={orderPinata3}, pinata3={pinata3}");
        }

        if (changed)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("✓ All OrderData assets configured successfully!");
            Debug.Log("➜ You can now delete Order_Energy3_B.asset if you want (it's no longer needed)");
            Debug.Log("➜ Use: Order_Energy3, Order_Gem3, Order_Pinata3 in your UISetup component");
        }
    }
}
