using UnityEngine;
using UnityEditor;

/// <summary>
/// Improved character setup using Humanoid Avatar bone mapping
/// Works with Mixamo and other humanoid characters
/// </summary>
public class SetupCharacterFromFBX_v2
{
    [MenuItem("Tools/Setup Character (Humanoid)")]
    public static void SetupCharacter()
    {
        Debug.Log("=== Starting Humanoid Character Setup ===");

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
        if (animator == null)
        {
            Debug.LogError("⚠ No Animator found on character!");
            Object.DestroyImmediate(characterInstance);
            return;
        }

        Debug.Log($"✓ Found Animator on: {animator.gameObject.name}");

        // If the animator is on a child, we need to work with the root
        GameObject characterRoot = characterInstance;

        // Add CharacterEquipmentManager to the root
        CharacterEquipmentManager equipManager = characterRoot.GetComponent<CharacterEquipmentManager>();
        if (equipManager == null)
        {
            equipManager = characterRoot.AddComponent<CharacterEquipmentManager>();
            Debug.Log("✓ Added CharacterEquipmentManager to root");
        }

        // Add CharacterAnimationController to the root
        CharacterAnimationController animController = characterRoot.GetComponent<CharacterAnimationController>();
        if (animController == null)
        {
            animController = characterRoot.AddComponent<CharacterAnimationController>();
            Debug.Log("✓ Added CharacterAnimationController to root");
        }

        // Assign animator to animation controller
        SerializedObject animSO = new SerializedObject(animController);
        animSO.FindProperty("animator").objectReferenceValue = animator;
        animSO.ApplyModifiedProperties();
        Debug.Log("✓ Assigned Animator to CharacterAnimationController");

        // Use Unity's Humanoid Avatar to find bones
        if (animator.avatar != null && animator.avatar.isHuman)
        {
            Debug.Log("✓ Character has Humanoid Avatar!");

            // Get bone transforms using HumanBodyBones enum
            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);

            Debug.Log($"Humanoid bones - RightHand: {rightHand != null}, LeftHand: {leftHand != null}, Head: {head != null}, Spine: {spine != null}");

            // Assign bones to equipment manager
            SerializedObject equipSO = new SerializedObject(equipManager);
            if (rightHand != null)
            {
                equipSO.FindProperty("rightHandSlot").objectReferenceValue = rightHand;
                Debug.Log($"✓ Assigned RightHand bone: {rightHand.name}");
            }
            if (leftHand != null)
            {
                equipSO.FindProperty("leftHandSlot").objectReferenceValue = leftHand;
                Debug.Log($"✓ Assigned LeftHand bone: {leftHand.name}");
            }
            if (head != null)
            {
                equipSO.FindProperty("headSlot").objectReferenceValue = head;
                Debug.Log($"✓ Assigned Head bone: {head.name}");
            }
            if (spine != null)
            {
                equipSO.FindProperty("backSlot").objectReferenceValue = spine;
                Debug.Log($"✓ Assigned Spine bone: {spine.name}");
            }
            equipSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("⚠ Character doesn't have a Humanoid Avatar configured!");
        }

        // Save as prefab
        string prefabPath = "Assets/Models/Character/Girl_Character.prefab";

        // Check if prefab already exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject prefab;

        if (existingPrefab != null)
        {
            // Update existing prefab
            prefab = PrefabUtility.SaveAsPrefabAsset(characterRoot, prefabPath);
            Debug.Log($"✓ Updated existing character prefab at: {prefabPath}");
        }
        else
        {
            // Create new prefab
            prefab = PrefabUtility.SaveAsPrefabAsset(characterRoot, prefabPath);
            Debug.Log($"✓ Created new character prefab at: {prefabPath}");
        }

        // Clean up scene instance
        Object.DestroyImmediate(characterRoot);

        // Update CharacterViewController to use this prefab
        CharacterViewController viewController = Object.FindFirstObjectByType<CharacterViewController>();
        if (viewController != null)
        {
            SerializedObject vcSO = new SerializedObject(viewController);
            vcSO.FindProperty("characterPrefab").objectReferenceValue = prefab;
            vcSO.ApplyModifiedProperties();
            Debug.Log("✓ Assigned character prefab to CharacterViewController");
        }
        else
        {
            Debug.LogWarning("⚠ CharacterViewController not found in scene");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("=== Humanoid Character Setup Complete! ===");
    }
}
