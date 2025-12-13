using UnityEngine;
using UnityEditor;

/// <summary>
/// Setup character with Generic rig - finds bones by searching hierarchy
/// Works with any character model regardless of bone naming
/// </summary>
public class SetupGenericCharacter
{
    [MenuItem("Tools/Setup Character (Generic Rig)")]
    public static void SetupCharacter()
    {
        Debug.Log("=== Starting Generic Character Setup ===");

        // Load the character FBX
        GameObject characterFBX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");

        if (characterFBX == null)
        {
            Debug.LogError("Could not find character FBX!");
            return;
        }

        Debug.Log($"✓ Found character FBX: {characterFBX.name}");

        // Instantiate the character in the scene
        GameObject characterInstance = PrefabUtility.InstantiatePrefab(characterFBX) as GameObject;
        characterInstance.name = "Girl_Character";

        // Get the Animator component
        Animator animator = characterInstance.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            Debug.Log($"✓ Found Animator on: {animator.gameObject.name}");
        }

        // Add CharacterEquipmentManager to the root
        CharacterEquipmentManager equipManager = characterInstance.GetComponent<CharacterEquipmentManager>();
        if (equipManager == null)
        {
            equipManager = characterInstance.AddComponent<CharacterEquipmentManager>();
            Debug.Log("✓ Added CharacterEquipmentManager");
        }

        // Add CharacterAnimationController to the root
        CharacterAnimationController animController = characterInstance.GetComponent<CharacterAnimationController>();
        if (animController == null)
        {
            animController = characterInstance.AddComponent<CharacterAnimationController>();
            Debug.Log("✓ Added CharacterAnimationController");
        }

        // Assign animator to animation controller
        if (animator != null)
        {
            SerializedObject animSO = new SerializedObject(animController);
            animSO.FindProperty("animator").objectReferenceValue = animator;
            animSO.ApplyModifiedProperties();
            Debug.Log("✓ Assigned Animator to CharacterAnimationController");
        }

        // Search for hand bones by common naming patterns
        Debug.Log("Searching for bones in hierarchy...");

        Transform[] allTransforms = characterInstance.GetComponentsInChildren<Transform>();
        Transform rightHand = FindBoneByPattern(allTransforms, new[] { "right", "hand", "r_hand", "rhand" });
        Transform leftHand = FindBoneByPattern(allTransforms, new[] { "left", "hand", "l_hand", "lhand" });
        Transform head = FindBoneByPattern(allTransforms, new[] { "head" });
        Transform spine = FindBoneByPattern(allTransforms, new[] { "spine", "back", "chest" });

        Debug.Log($"Bones found - RightHand: {rightHand != null} ({rightHand?.name}), LeftHand: {leftHand != null} ({leftHand?.name}), Head: {head != null} ({head?.name}), Spine: {spine != null} ({spine?.name})");

        // If we didn't find bones, list all transforms so user can see what's available
        if (rightHand == null && leftHand == null)
        {
            Debug.LogWarning("Could not find hand bones automatically. Here are all available transforms:");
            foreach (Transform t in allTransforms)
            {
                if (t != characterInstance.transform) // Skip root
                {
                    Debug.Log($"  - {t.name}");
                }
            }
        }

        // Assign bones to equipment manager
        SerializedObject equipSO = new SerializedObject(equipManager);
        if (rightHand != null)
        {
            equipSO.FindProperty("rightHandSlot").objectReferenceValue = rightHand;
            Debug.Log($"✓ Assigned RightHand: {rightHand.name}");
        }
        if (leftHand != null)
        {
            equipSO.FindProperty("leftHandSlot").objectReferenceValue = leftHand;
            Debug.Log($"✓ Assigned LeftHand: {leftHand.name}");
        }
        if (head != null)
        {
            equipSO.FindProperty("headSlot").objectReferenceValue = head;
            Debug.Log($"✓ Assigned Head: {head.name}");
        }
        if (spine != null)
        {
            equipSO.FindProperty("backSlot").objectReferenceValue = spine;
            Debug.Log($"✓ Assigned Back/Spine: {spine.name}");
        }
        equipSO.ApplyModifiedProperties();

        // Save as prefab
        string prefabPath = "Assets/Models/Character/Girl_Character.prefab";
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject prefab;

        if (existingPrefab != null)
        {
            prefab = PrefabUtility.SaveAsPrefabAsset(characterInstance, prefabPath);
            Debug.Log($"✓ Updated existing character prefab");
        }
        else
        {
            prefab = PrefabUtility.SaveAsPrefabAsset(characterInstance, prefabPath);
            Debug.Log($"✓ Created new character prefab");
        }

        // Clean up scene instance
        Object.DestroyImmediate(characterInstance);

        // Update CharacterViewController
        CharacterViewController viewController = Object.FindFirstObjectByType<CharacterViewController>();
        if (viewController != null)
        {
            SerializedObject vcSO = new SerializedObject(viewController);
            vcSO.FindProperty("characterPrefab").objectReferenceValue = prefab;
            vcSO.ApplyModifiedProperties();
            Debug.Log("✓ Assigned character prefab to CharacterViewController");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("=== Generic Character Setup Complete! ===");

        if (rightHand == null)
        {
            Debug.LogWarning("⚠ Could not find RightHand bone automatically!");
            Debug.LogWarning("You may need to manually assign the hand bones in the Girl_Character prefab.");
        }
    }

    private static Transform FindBoneByPattern(Transform[] transforms, string[] patterns)
    {
        foreach (Transform t in transforms)
        {
            string nameLower = t.name.ToLower();

            // Check if ALL patterns are in the name (for multi-word matches like "right hand")
            bool matchesAll = true;
            foreach (string pattern in patterns)
            {
                if (!nameLower.Contains(pattern.ToLower()))
                {
                    matchesAll = false;
                    break;
                }
            }

            if (matchesAll)
            {
                return t;
            }
        }

        return null;
    }
}
