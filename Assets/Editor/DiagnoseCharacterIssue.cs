using UnityEngine;
using UnityEditor;

public class DiagnoseCharacterIssue : EditorWindow
{
    [MenuItem("Tools/Diagnose Character & Axe")]
    public static void Diagnose()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== CHARACTER & AXE DIAGNOSTIC ===");

        // Find CharacterViewController
        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC == null)
        {
            Debug.LogError("❌ CharacterViewController not found!");
            return;
        }

        Debug.Log($"✓ CharacterViewController found");
        Debug.Log($"  - characterRoot: {charVC.GetCharacterWorldPosition()}");
        Debug.Log($"  - characterCamera: {charVC.characterCamera != null}");
        Debug.Log($"  - characterRenderTexture: {charVC.characterRenderTexture != null}");
        Debug.Log($"  - characterDisplayUI: {charVC.characterDisplayUI != null}");

        // Get equipment manager
        CharacterEquipmentManager equipMgr = charVC.GetEquipmentManager();
        if (equipMgr == null)
        {
            Debug.LogError("❌ CharacterEquipmentManager not found!");
            return;
        }

        Debug.Log($"✓ CharacterEquipmentManager found on: {equipMgr.gameObject.name}");
        Debug.Log($"  - GameObject layer: {LayerMask.LayerToName(equipMgr.gameObject.layer)}");
        Debug.Log($"  - rightHandSlot: {equipMgr.rightHandSlot != null}");
        
        if (equipMgr.rightHandSlot != null)
        {
            Debug.Log($"  - rightHandSlot position: {equipMgr.rightHandSlot.position}");
            Debug.Log($"  - rightHandSlot childCount: {equipMgr.rightHandSlot.childCount}");
            
            // Check for equipped items
            for (int i = 0; i < equipMgr.rightHandSlot.childCount; i++)
            {
                Transform child = equipMgr.rightHandSlot.GetChild(i);
                Debug.Log($"  - Child {i}: {child.name}, layer: {LayerMask.LayerToName(child.gameObject.layer)}, active: {child.gameObject.activeSelf}");
                Debug.Log($"    Position: {child.localPosition}, Rotation: {child.localRotation.eulerAngles}, Scale: {child.localScale}");
            }
        }

        // Check camera settings
        if (charVC.characterCamera != null)
        {
            Camera cam = charVC.characterCamera;
            Debug.Log($"✓ Character Camera:");
            Debug.Log($"  - Culling mask: {LayerMask.LayerToName(cam.cullingMask)}");
            Debug.Log($"  - Position: {cam.transform.position}");
            Debug.Log($"  - Target texture: {cam.targetTexture != null}");
            Debug.Log($"  - Enabled: {cam.enabled}");
        }

        // Check Order_Energy3
        OrderManager orderMgr = FindFirstObjectByType<OrderManager>();
        if (orderMgr != null && orderMgr.availableOrders != null)
        {
            foreach (var order in orderMgr.availableOrders)
            {
                if (order != null && order.name == "Order_Energy3")
                {
                    Debug.Log($"✓ Order_Energy3:");
                    Debug.Log($"  - itemType: {order.itemType}");
                    Debug.Log($"  - equipmentSlot: {order.equipmentSlot}");
                    Debug.Log($"  - itemPrefab3D: {order.itemPrefab3D != null}");
                    if (order.itemPrefab3D != null)
                    {
                        Debug.Log($"  - Prefab name: {order.itemPrefab3D.name}");
                    }
                    Debug.Log($"  - Position offset: {order.equipmentPositionOffset}");
                    Debug.Log($"  - Rotation offset: {order.equipmentRotationOffset}");
                    Debug.Log($"  - Scale: {order.equipmentScale}");
                }
            }
        }

        Debug.Log("=== END DIAGNOSTIC ===");
    }
}
