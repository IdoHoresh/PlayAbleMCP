using UnityEngine;
using UnityEditor;

public class InspectSkinnedMeshBones
{
    [MenuItem("Tools/Inspect Skinned Mesh Bones")]
    public static void InspectBones()
    {
        // Load the FBX
        GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");
        
        if (fbx == null)
        {
            Debug.LogError("FBX not found!");
            return;
        }

        Debug.Log("=== Inspecting FBX Skinned Mesh Renderer ===");
        
        // Instantiate temporarily to inspect
        GameObject instance = PrefabUtility.InstantiatePrefab(fbx) as GameObject;
        
        if (instance == null)
        {
            Debug.LogError("Failed to instantiate FBX!");
            return;
        }

        try
        {
            // Find all SkinnedMeshRenderer components
            SkinnedMeshRenderer[] renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            
            Debug.Log($"Found {renderers.Length} SkinnedMeshRenderer(s)");
            
            foreach (SkinnedMeshRenderer smr in renderers)
            {
                Debug.Log($"\n=== Skinned Mesh: {smr.gameObject.name} ===");
                Debug.Log($"Root Bone: {(smr.rootBone != null ? smr.rootBone.name : "None")}");
                Debug.Log($"Total Bones: {smr.bones.Length}");
                Debug.Log("\nBone List:");
                
                for (int i = 0; i < smr.bones.Length; i++)
                {
                    if (smr.bones[i] != null)
                    {
                        string path = GetFullPath(smr.bones[i], instance.transform);
                        Debug.Log($"  [{i}] {smr.bones[i].name} (path: {path})");
                    }
                }
            }
            
            // Also list all transforms in hierarchy
            Debug.Log("\n=== All Transforms in Hierarchy ===");
            Transform[] allTransforms = instance.GetComponentsInChildren<Transform>(true);
            Debug.Log($"Total transforms: {allTransforms.Length}");
            
            foreach (Transform t in allTransforms)
            {
                string indent = GetIndent(t, instance.transform);
                Debug.Log($"{indent}{t.name}");
            }
            
            // Search for key bones
            Debug.Log("\n=== Searching for Equipment Attachment Points ===");
            
            foreach (Transform t in allTransforms)
            {
                string nameLower = t.name.ToLower();
                
                if (nameLower.Contains("hand"))
                {
                    Debug.Log($"🖐 Hand bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                }
                
                if (nameLower.Contains("head") || nameLower.Contains("skull") || nameLower.Contains("neck"))
                {
                    Debug.Log($"🎭 Head/Neck bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                }
                
                if (nameLower.Contains("spine") || nameLower.Contains("chest") || nameLower.Contains("torso") || nameLower.Contains("back"))
                {
                    Debug.Log($"🦴 Spine/Back bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                }
                
                if (nameLower.Contains("arm") || nameLower.Contains("shoulder") || nameLower.Contains("elbow") || nameLower.Contains("wrist"))
                {
                    Debug.Log($"💪 Arm bone: {t.name} (path: {GetFullPath(t, instance.transform)})");
                }
            }
        }
        finally
        {
            // Clean up
            Object.DestroyImmediate(instance);
        }
    }

    private static string GetIndent(Transform t, Transform root)
    {
        int depth = 0;
        Transform current = t.parent;
        
        while (current != null && current != root.parent)
        {
            depth++;
            current = current.parent;
        }
        
        return new string(' ', depth * 2) + (depth > 0 ? "└─ " : "");
    }

    private static string GetFullPath(Transform t, Transform root)
    {
        string path = t.name;
        Transform current = t.parent;
        
        while (current != null && current != root.parent)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
