using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to set up the character from the imported FBX file
/// Creates a prefab with all necessary components configured
/// </summary>
public class SetupCharacterFromFBX
{
    [MenuItem("Tools/Setup Character from FBX")]
    public static void SetupCharacter()
    {
        Debug.Log("=== Starting Character Setup ===");

        // Load the character FBX
        GameObject characterFBX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");

        if (characterFBX == null)
        {
            Debug.LogError("Could not find character FBX at Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");
            return;
        }

        Debug.Log($"✓ Found character FBX: {characterFBX.name}");

        // Instantiate the character in the scene
        GameObject characterInstance = PrefabUtility.InstantiatePrefab(characterFBX) as GameObject;
        characterInstance.name = "Girl_Character";

        // Add CharacterEquipmentManager
        CharacterEquipmentManager equipManager = characterInstance.GetComponent<CharacterEquipmentManager>();
        if (equipManager == null)
        {
            equipManager = characterInstance.AddComponent<CharacterEquipmentManager>();
            Debug.Log("✓ Added CharacterEquipmentManager");
        }

        // Add CharacterAnimationController
        CharacterAnimationController animController = characterInstance.GetComponent<CharacterAnimationController>();
        if (animController == null)
        {
            animController = characterInstance.AddComponent<CharacterAnimationController>();
            Debug.Log("✓ Added CharacterAnimationController");
        }

        // Try to find the Animator component
        Animator animator = characterInstance.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            Debug.Log($"✓ Found Animator on: {animator.gameObject.name}");

            // Assign animator to animation controller via reflection (since it's a public field)
            SerializedObject so = new SerializedObject(animController);
            so.FindProperty("animator").objectReferenceValue = animator;
            so.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("⚠ No Animator found on character");
        }

        // Find bone attachment points
        Transform rightHandBone = FindBoneInHierarchy(characterInstance.transform, "RightHand");
        Transform leftHandBone = FindBoneInHierarchy(characterInstance.transform, "LeftHand");
        Transform headBone = FindBoneInHierarchy(characterInstance.transform, "Head");
        Transform spineBone = FindBoneInHierarchy(characterInstance.transform, "Spine");

        Debug.Log($"Bones found - RightHand: {rightHandBone != null}, LeftHand: {leftHandBone != null}, Head: {headBone != null}, Spine: {spineBone != null}");

        // Assign attachment points to equipment manager
        SerializedObject equipSO = new SerializedObject(equipManager);
        if (rightHandBone != null)
        {
            equipSO.FindProperty("rightHandSlot").objectReferenceValue = rightHandBone;
            Debug.Log($"✓ Assigned RightHand bone: {rightHandBone.name}");
        }
        if (leftHandBone != null)
        {
            equipSO.FindProperty("leftHandSlot").objectReferenceValue = leftHandBone;
            Debug.Log($"✓ Assigned LeftHand bone: {leftHandBone.name}");
        }
        if (headBone != null)
        {
            equipSO.FindProperty("headSlot").objectReferenceValue = headBone;
            Debug.Log($"✓ Assigned Head bone: {headBone.name}");
        }
        if (spineBone != null)
        {
            equipSO.FindProperty("backSlot").objectReferenceValue = spineBone;
            Debug.Log($"✓ Assigned Spine bone (for back slot): {spineBone.name}");
        }
        equipSO.ApplyModifiedProperties();

        // Save as prefab
        string prefabPath = "Assets/Models/Character/Girl_Character.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(characterInstance, prefabPath);

        Debug.Log($"✓ Saved character prefab to: {prefabPath}");

        // Clean up scene instance
        Object.DestroyImmediate(characterInstance);

        // Now update the CharacterViewController to use this prefab
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
            Debug.LogWarning("⚠ CharacterViewController not found in scene. Manually assign the prefab.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("=== Character Setup Complete! ===");
        Debug.Log("Next steps:");
        Debug.Log("1. Create an Animator Controller for the character animations");
        Debug.Log("2. Assign animation clips to the controller");
        Debug.Log("3. Enter Play mode to see the character");
    }

    private static Transform FindBoneInHierarchy(Transform root, string boneName)
    {
        // Try exact match first
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name == boneName)
                return child;
        }

        // Try contains match (for names like "mixamorig:RightHand")
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains(boneName))
                return child;
        }

        return null;
    }
}
