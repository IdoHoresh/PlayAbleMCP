using UnityEngine;
using UnityEditor;

/// <summary>
/// Remove the TestCharacter from the scene so the Girl_Character can be used
/// </summary>
public class RemoveTestCharacter
{
    [MenuItem("Tools/Remove TestCharacter from Scene")]
    public static void RemoveTest()
    {
        Debug.Log("=== Removing TestCharacter from Scene ===");

        // Find all CharacterEquipmentManager components in the scene
        CharacterEquipmentManager[] allManagers = Object.FindObjectsByType<CharacterEquipmentManager>(FindObjectsSortMode.None);

        foreach (CharacterEquipmentManager manager in allManagers)
        {
            if (manager.gameObject.name.Contains("TestCharacter"))
            {
                Debug.Log($"Found TestCharacter: {manager.gameObject.name}");
                Object.DestroyImmediate(manager.gameObject);
                Debug.Log("✓ Removed TestCharacter from scene");

                EditorUtility.SetDirty(manager.gameObject.scene.GetRootGameObjects()[0]);
                return;
            }
        }

        Debug.LogWarning("No TestCharacter found in scene");
    }
}
