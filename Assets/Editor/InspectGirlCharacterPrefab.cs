using UnityEngine;
using UnityEditor;

public class InspectGirlCharacterPrefab
{
    [MenuItem("Tools/Inspect Girl Character Prefab")]
    public static void InspectPrefab()
    {
        // Load the Girl_Character prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Girl_Character.prefab");
        
        if (prefab == null)
        {
            Debug.LogError("Girl_Character prefab not found!");
            return;
        }

        Debug.Log("=== Inspecting Girl_Character Prefab ===");
        
        // Instantiate the prefab to inspect its runtime structure
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        
        if (instance == null)
        {
            Debug.LogError("Failed to instantiate prefab!");
            return;
        }

        try
        {
            Debug.Log($"Prefab Root: {instance.name}");
            
            // Get all transforms
            Transform[] allTransforms = instance.GetComponentsInChildren<Transform>(true);
            Debug.Log($"Total transforms in prefab: {allTransforms.Length}\n");
            
            // Print full hierarchy
            Debug.Log("=== Full Hierarchy ===");
            PrintHierarchy(instance.transform, 0);
            
            // Find SkinnedMeshRenderer
            Debug.Log("\n=== Skinned Mesh Renderers ===");
            SkinnedMeshRenderer[] renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            Debug.Log($"Found {renderers.Length} SkinnedMeshRenderer(s)");
            
            foreach (SkinnedMeshRenderer smr in renderers)
            {
                Debug.Log($"\nRenderer on: {smr.gameObject.name}");
                Debug.Log($"  Root Bone: {(smr.rootBone != null ? smr.rootBone.name : "None")}");
                Debug.Log($"  Bones Count: {smr.bones.Length}");
                
                if (smr.bones.Length > 0)
                {
                    Debug.Log("  Bone List:");
                    for (int i = 0; i < Mathf.Min(smr.bones.Length, 50); i++) // Limit to first 50
                    {
                        if (smr.bones[i] != null)
                        {
                            Debug.Log($"    [{i}] {smr.bones[i].name}");
                        }
                    }
                    if (smr.bones.Length > 50)
                    {
                        Debug.Log($"    ... and {smr.bones.Length - 50} more bones");
                    }
                }
            }
            
            // Find MeshRenderer
            Debug.Log("\n=== Mesh Renderers ===");
            MeshRenderer[] meshRenderers = instance.GetComponentsInChildren<MeshRenderer>(true);
            Debug.Log($"Found {meshRenderers.Length} MeshRenderer(s)");
            foreach (MeshRenderer mr in meshRenderers)
            {
                Debug.Log($"  - {mr.gameObject.name}");
            }
            
            // Find Animator
            Debug.Log("\n=== Animator ===");
            Animator animator = instance.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"Animator found on {animator.gameObject.name}");
                Debug.Log($"  Avatar: {(animator.avatar != null ? animator.avatar.name : "None")}");
                Debug.Log($"  Is Human: {animator.avatar != null && animator.avatar.isHuman}");
                Debug.Log($"  Is Valid: {animator.avatar != null && animator.avatar.isValid}");
                
                if (animator.avatar != null && animator.avatar.isHuman)
                {
                    Debug.Log("\n  Humanoid Bones:");
                    TryLogBone(animator, HumanBodyBones.RightHand, "Right Hand");
                    TryLogBone(animator, HumanBodyBones.LeftHand, "Left Hand");
                    TryLogBone(animator, HumanBodyBones.Head, "Head");
                    TryLogBone(animator, HumanBodyBones.Spine, "Spine");
                    TryLogBone(animator, HumanBodyBones.Chest, "Chest");
                    TryLogBone(animator, HumanBodyBones.UpperChest, "Upper Chest");
                }
            }
            else
            {
                Debug.LogWarning("No Animator found!");
            }
            
            // Search for key bones by name
            Debug.Log("\n=== Searching for Equipment Attachment Points ===");
            int foundCount = 0;
            
            foreach (Transform t in allTransforms)
            {
                string nameLower = t.name.ToLower();
                
                if (nameLower.Contains("hand") || nameLower.Contains("wrist"))
                {
                    Debug.Log($"🖐 Hand/Wrist bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                    foundCount++;
                }
                
                if (nameLower.Contains("head") || nameLower.Contains("skull"))
                {
                    Debug.Log($"🎭 Head bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                    foundCount++;
                }
                
                if (nameLower.Contains("spine") || nameLower.Contains("chest") || nameLower.Contains("torso"))
                {
                    Debug.Log($"🦴 Spine/Chest bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                    foundCount++;
                }
            }
            
            if (foundCount == 0)
            {
                Debug.LogWarning("⚠ No obvious equipment attachment points found by name!");
                Debug.Log("Here are ALL transform names for reference:");
                foreach (Transform t in allTransforms)
                {
                    Debug.Log($"  - {t.name}");
                }
            }
        }
        finally
        {
            // Clean up
            Object.DestroyImmediate(instance);
        }
    }

    private static void PrintHierarchy(Transform t, int depth)
    {
        string indent = new string(' ', depth * 2);
        string prefix = depth > 0 ? "└─ " : "";
        Debug.Log($"{indent}{prefix}{t.name}");
        
        foreach (Transform child in t)
        {
            PrintHierarchy(child, depth + 1);
        }
    }

    private static void TryLogBone(Animator animator, HumanBodyBones bone, string boneName)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform != null)
        {
            Debug.Log($"    {boneName}: {boneTransform.name}");
        }
    }

    private static string GetFullPath(Transform t, Transform root)
    {
        string path = t.name;
        Transform current = t.parent;
        
        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
