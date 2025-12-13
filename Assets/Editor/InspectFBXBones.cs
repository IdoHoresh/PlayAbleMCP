using UnityEngine;
using UnityEditor;

public class InspectFBXBones
{
    [MenuItem("Tools/Inspect Girl FBX Bone Structure")]
    public static void InspectBones()
    {
        // Load the FBX directly
        GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");
        
        if (fbx == null)
        {
            Debug.LogError("FBX not found!");
            return;
        }

        Debug.Log("=== FBX Bone Structure ===");
        Debug.Log($"Root: {fbx.name}");
        
        // Get all transforms
        Transform[] allTransforms = fbx.GetComponentsInChildren<Transform>(true);
        Debug.Log($"Total bones/transforms: {allTransforms.Length}");
        Debug.Log("");
        
        // Print hierarchy
        foreach (Transform t in allTransforms)
        {
            string indent = GetIndent(t, fbx.transform);
            Debug.Log($"{indent}{t.name}");
        }
        
        Debug.Log("");
        Debug.Log("=== Searching for Equipment Bones ===");
        
        // Try to identify key bones
        foreach (Transform t in allTransforms)
        {
            string nameLower = t.name.ToLower();
            
            if (nameLower.Contains("hand"))
            {
                Debug.Log($"🖐 Hand bone found: {t.name} (full path: {GetFullPath(t, fbx.transform)})");
            }
            
            if (nameLower.Contains("head") || nameLower.Contains("skull"))
            {
                Debug.Log($"🎭 Head bone found: {t.name} (full path: {GetFullPath(t, fbx.transform)})");
            }
            
            if (nameLower.Contains("spine") || nameLower.Contains("chest") || nameLower.Contains("torso"))
            {
                Debug.Log($"🦴 Spine/Chest bone found: {t.name} (full path: {GetFullPath(t, fbx.transform)})");
            }
        }
    }

    private static string GetIndent(Transform t, Transform root)
    {
        int depth = 0;
        Transform current = t.parent;
        
        while (current != null && current != root)
        {
            depth++;
            current = current.parent;
        }
        
        return new string(' ', depth * 2) + "└─ ";
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
