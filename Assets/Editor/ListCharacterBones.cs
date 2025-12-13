using UnityEngine;
using UnityEditor;

/// <summary>
/// Debug tool to list all bones in the character FBX
/// Helps us find the correct bone names
/// </summary>
public class ListCharacterBones
{
    [MenuItem("Tools/List Character Bones")]
    public static void ListBones()
    {
        Debug.Log("=== Listing Character Bones ===");

        // Load the character FBX
        GameObject characterFBX = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx");

        if (characterFBX == null)
        {
            Debug.LogError("Could not find character FBX!");
            return;
        }

        // Instantiate temporarily to inspect
        GameObject instance = PrefabUtility.InstantiatePrefab(characterFBX) as GameObject;

        Debug.Log($"Character root: {instance.name}");
        Debug.Log("All transforms in hierarchy:");
        Debug.Log("---");

        int count = 0;
        foreach (Transform child in instance.GetComponentsInChildren<Transform>())
        {
            count++;
            string indent = new string(' ', GetDepth(child) * 2);
            Debug.Log($"{indent}{child.name} (depth: {GetDepth(child)})");
        }

        Debug.Log($"---");
        Debug.Log($"Total transforms found: {count}");

        // Clean up
        Object.DestroyImmediate(instance);

        Debug.Log("=== Done ===");
    }

    private static int GetDepth(Transform t)
    {
        int depth = 0;
        while (t.parent != null)
        {
            depth++;
            t = t.parent;
        }
        return depth;
    }
}
