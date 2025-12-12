using UnityEngine;
using UnityEditor;

public class FixCharacterSetup : EditorWindow
{
    [MenuItem("Tools/Fix Character Setup")]
    public static void Fix()
    {
        // Find CharacterViewController in scene
        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        
        if (charVC != null)
        {
            // Clear the characterPrefab field to prevent spawning
            charVC.characterPrefab = null;
            EditorUtility.SetDirty(charVC);
            Debug.Log("✓ Cleared CharacterViewController.characterPrefab");
        }
        else
        {
            Debug.LogWarning("CharacterViewController not found in scene");
        }
        
        // Find all TestCharacter instances
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int characterCount = 0;
        GameObject firstCharacter = null;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TestCharacter"))
            {
                characterCount++;
                if (firstCharacter == null)
                {
                    firstCharacter = obj;
                }
                else
                {
                    // Delete duplicate
                    Debug.Log($"✓ Deleted duplicate character: {obj.name}");
                    DestroyImmediate(obj);
                }
            }
        }
        
        if (characterCount > 0)
        {
            Debug.Log($"✓ Found {characterCount} character(s). Kept first one, deleted duplicates.");
            
            // Make sure the remaining character is in the scene root (not in a prefab instance)
            if (firstCharacter != null && firstCharacter.transform.parent != null)
            {
                string parentName = firstCharacter.transform.parent.name;
                if (parentName == "CharacterRoot")
                {
                    Debug.Log("✓ Character is already under CharacterRoot (correct)");
                }
                else
                {
                    Debug.LogWarning($"Character parent is '{parentName}' - this might be okay");
                }
            }
        }
        else
        {
            Debug.LogWarning("No TestCharacter found in scene!");
        }
        
        Debug.Log("✓ Character setup fixed! Stop Play mode and restart to see changes.");
    }
}
