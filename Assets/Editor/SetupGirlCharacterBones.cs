using UnityEngine;
using UnityEditor;

public class SetupGirlCharacterBones
{
    [MenuItem("Tools/Setup Girl Character Bones")]
    public static void SetupBones()
    {
        // Load the Girl_Character prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Girl_Character.prefab");
        
        if (prefab == null)
        {
            Debug.LogError("Girl_Character prefab not found!");
            return;
        }

        // Load the prefab for editing
        string prefabPath = "Assets/Models/Character/Girl_Character.prefab";
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        if (prefabInstance == null)
        {
            Debug.LogError("Failed to load prefab for editing!");
            return;
        }

        try
        {
            // Get the CharacterEquipmentManager component
            CharacterEquipmentManager equipmentManager = prefabInstance.GetComponent<CharacterEquipmentManager>();
            
            if (equipmentManager == null)
            {
                Debug.LogError("CharacterEquipmentManager not found on prefab!");
                return;
            }

            // Get all transforms in the character hierarchy
            Transform[] allTransforms = prefabInstance.GetComponentsInChildren<Transform>();
            
            Debug.Log($"Found {allTransforms.Length} transforms in character hierarchy:");
            
            // List all bone names for debugging
            foreach (Transform t in allTransforms)
            {
                Debug.Log($"  - {t.name} (path: {GetTransformPath(t, prefabInstance.transform)})");
            }

            // Try to find bones by common naming patterns
            Transform rightHand = FindBone(allTransforms, new[] { "hand", "right" }, new[] { "left" });
            Transform leftHand = FindBone(allTransforms, new[] { "hand", "left" }, new[] { "right" });
            Transform head = FindBone(allTransforms, new[] { "head" }, new string[0]);
            Transform spine = FindBone(allTransforms, new[] { "spine", "chest", "body" }, new string[0]);

            // If we still don't have bones, try alternative patterns
            if (rightHand == null)
                rightHand = FindBone(allTransforms, new[] { "r_hand", "rhand", "hand_r", "handr" }, new string[0]);
            
            if (leftHand == null)
                leftHand = FindBone(allTransforms, new[] { "l_hand", "lhand", "hand_l", "handl" }, new string[0]);

            // Assign bones to equipment manager using SerializedObject
            SerializedObject so = new SerializedObject(equipmentManager);
            
            if (rightHand != null)
            {
                so.FindProperty("rightHandSlot").objectReferenceValue = rightHand;
                Debug.Log($"✓ Assigned Right Hand: {rightHand.name}");
            }
            else
            {
                Debug.LogWarning("✗ Right Hand bone not found!");
            }

            if (leftHand != null)
            {
                so.FindProperty("leftHandSlot").objectReferenceValue = leftHand;
                Debug.Log($"✓ Assigned Left Hand: {leftHand.name}");
            }
            else
            {
                Debug.LogWarning("✗ Left Hand bone not found!");
            }

            if (head != null)
            {
                so.FindProperty("headSlot").objectReferenceValue = head;
                Debug.Log($"✓ Assigned Head: {head.name}");
            }
            else
            {
                Debug.LogWarning("✗ Head bone not found!");
            }

            if (spine != null)
            {
                so.FindProperty("backSlot").objectReferenceValue = spine;
                so.FindProperty("bodySlot").objectReferenceValue = spine;
                Debug.Log($"✓ Assigned Spine/Body: {spine.name}");
            }
            else
            {
                Debug.LogWarning("✗ Spine bone not found!");
            }

            so.ApplyModifiedProperties();

            // Save the prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            Debug.Log("✓✓✓ Girl_Character prefab updated with bone assignments!");
        }
        finally
        {
            // Always unload the prefab contents
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }

        AssetDatabase.Refresh();
    }

    private static Transform FindBone(Transform[] transforms, string[] mustContain, string[] mustNotContain)
    {
        foreach (Transform t in transforms)
        {
            string nameLower = t.name.ToLower();
            
            // Check if name contains all required patterns
            bool hasAll = true;
            foreach (string pattern in mustContain)
            {
                if (!nameLower.Contains(pattern.ToLower()))
                {
                    hasAll = false;
                    break;
                }
            }
            
            if (!hasAll) continue;
            
            // Check if name doesn't contain excluded patterns
            bool hasExcluded = false;
            foreach (string pattern in mustNotContain)
            {
                if (nameLower.Contains(pattern.ToLower()))
                {
                    hasExcluded = true;
                    break;
                }
            }
            
            if (!hasExcluded)
            {
                return t;
            }
        }
        
        return null;
    }

    private static string GetTransformPath(Transform transform, Transform root)
    {
        string path = transform.name;
        Transform current = transform.parent;
        
        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
