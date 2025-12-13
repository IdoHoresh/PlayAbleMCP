using UnityEngine;
using UnityEditor;

public class CreateManualAttachmentPoints
{
    [MenuItem("Tools/Create Manual Attachment Points")]
    public static void CreateAttachmentPoints()
    {
        // Load the Girl_Character prefab
        string prefabPath = "Assets/Models/Character/Girl_Character.prefab";
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        if (prefabInstance == null)
        {
            Debug.LogError("Failed to load Girl_Character prefab!");
            return;
        }

        try
        {
            Debug.Log("=== Creating Manual Attachment Points ===");
            Debug.Log("Note: Since this character has no bones, we're creating empty GameObjects as attachment points.");
            Debug.Log("You'll need to position these manually in the Scene view.");
            
            // Get the CharacterEquipmentManager
            CharacterEquipmentManager equipmentManager = prefabInstance.GetComponent<CharacterEquipmentManager>();
            
            if (equipmentManager == null)
            {
                Debug.LogError("CharacterEquipmentManager not found!");
                return;
            }

            // Create attachment point GameObjects
            Transform rightHand = CreateAttachmentPoint(prefabInstance.transform, "RightHandSlot", 
                new Vector3(0.15f, 1.2f, 0.05f));  // Right side, shoulder height, slightly forward
            
            Transform leftHand = CreateAttachmentPoint(prefabInstance.transform, "LeftHandSlot", 
                new Vector3(-0.15f, 1.2f, 0.05f)); // Left side, shoulder height, slightly forward
            
            Transform head = CreateAttachmentPoint(prefabInstance.transform, "HeadSlot", 
                new Vector3(0f, 1.7f, 0f));        // Top center
            
            Transform back = CreateAttachmentPoint(prefabInstance.transform, "BackSlot", 
                new Vector3(0f, 1.0f, -0.15f));    // Back center, mid height
            
            Transform body = CreateAttachmentPoint(prefabInstance.transform, "BodySlot", 
                new Vector3(0f, 1.0f, 0f));        // Center, mid height

            // Assign to equipment manager
            SerializedObject so = new SerializedObject(equipmentManager);
            so.FindProperty("rightHandSlot").objectReferenceValue = rightHand;
            so.FindProperty("leftHandSlot").objectReferenceValue = leftHand;
            so.FindProperty("headSlot").objectReferenceValue = head;
            so.FindProperty("backSlot").objectReferenceValue = back;
            so.FindProperty("bodySlot").objectReferenceValue = body;
            so.ApplyModifiedProperties();

            Debug.Log("✓ Created attachment points:");
            Debug.Log($"  - Right Hand: {rightHand.name}");
            Debug.Log($"  - Left Hand: {leftHand.name}");
            Debug.Log($"  - Head: {head.name}");
            Debug.Log($"  - Back: {back.name}");
            Debug.Log($"  - Body: {body.name}");
            Debug.Log("\n⚠ IMPORTANT: These are estimated positions!");
            Debug.Log("To adjust positions:");
            Debug.Log("1. Drag Girl_Character prefab into the scene");
            Debug.Log("2. Adjust the position of each attachment point");
            Debug.Log("3. Apply changes back to the prefab");

            // Save prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            Debug.Log("\n✓✓✓ Prefab saved with manual attachment points!");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }

        AssetDatabase.Refresh();
    }

    private static Transform CreateAttachmentPoint(Transform parent, string name, Vector3 localPosition)
    {
        // Check if it already exists
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            Debug.Log($"  Using existing: {name}");
            existing.localPosition = localPosition;
            return existing;
        }

        // Create new
        GameObject attachPoint = new GameObject(name);
        attachPoint.transform.SetParent(parent, false);
        attachPoint.transform.localPosition = localPosition;
        attachPoint.transform.localRotation = Quaternion.identity;
        attachPoint.transform.localScale = Vector3.one;

        Debug.Log($"  Created: {name} at {localPosition}");
        return attachPoint.transform;
    }
}
