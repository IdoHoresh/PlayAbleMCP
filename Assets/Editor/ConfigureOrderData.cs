using UnityEngine;
using UnityEditor;

public class ConfigureOrderData : EditorWindow
{
    [MenuItem("Tools/Configure Order Data")]
    public static void ConfigureOrders()
    {
        // Load ItemData assets
        ItemData energy3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Energy_Level3.asset");
        ItemData gem3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Gem_Level3.asset");
        ItemData pinata3 = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Pinata_Level3.asset");

        // Load OrderData assets
        OrderData orderEnergy3 = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Energy3.asset");
        OrderData orderPinata3 = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Pinata3.asset");
        OrderData orderEnergy3B = AssetDatabase.LoadAssetAtPath<OrderData>("Assets/Data/Orders/Order_Energy3_B.asset");

        bool changed = false;

        // Configure Order_Energy3 (requires 1x Energy Level 3)
        if (orderEnergy3 != null && energy3 != null)
        {
            orderEnergy3.requiredItem = energy3;
            orderEnergy3.quantity = 1;
            orderEnergy3.coinReward = 100;
            EditorUtility.SetDirty(orderEnergy3);
            Debug.Log("Configured Order_Energy3: 1x Energy Level 3, Reward: 100 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Energy3 or Energy_Level3. orderEnergy3={orderEnergy3}, energy3={energy3}");
        }

        // Configure Order_Pinata3 (requires 1x Pinata Level 3)
        if (orderPinata3 != null && pinata3 != null)
        {
            orderPinata3.requiredItem = pinata3;
            orderPinata3.quantity = 1;
            orderPinata3.coinReward = 150;
            EditorUtility.SetDirty(orderPinata3);
            Debug.Log("Configured Order_Pinata3: 1x Pinata Level 3, Reward: 150 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Pinata3 or Pinata_Level3. orderPinata3={orderPinata3}, pinata3={pinata3}");
        }

        // Configure Order_Energy3_B as Gem Level 3 (requires 1x Gem Level 3)
        if (orderEnergy3B != null && gem3 != null)
        {
            orderEnergy3B.requiredItem = gem3;
            orderEnergy3B.quantity = 1;
            orderEnergy3B.coinReward = 120;
            EditorUtility.SetDirty(orderEnergy3B);
            Debug.Log("Configured Order_Energy3_B: 1x Gem Level 3, Reward: 120 coins");
            changed = true;
        }
        else
        {
            Debug.LogError($"Failed to load Order_Energy3_B or Gem_Level3. orderEnergy3B={orderEnergy3B}, gem3={gem3}");
        }

        if (changed)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("✓ All OrderData assets configured successfully!");
        }
    }
}
