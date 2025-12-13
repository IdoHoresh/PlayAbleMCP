using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetCharacterTransformValues : EditorWindow
{
    [MenuItem("Tools/Set Character Transform Values")]
    public static void SetValues()
    {
        Debug.Log("=== Setting Character Transform Values ===");

        // Find CharacterViewController in the scene
        CharacterViewController controller = FindFirstObjectByType<CharacterViewController>();
        
        if (controller == null)
        {
            Debug.LogError("CharacterViewController not found!");
            return;
        }

        // Set the correct values
        controller.characterPosition = new Vector3(0f, 0f, 0f);
        controller.characterScale = new Vector3(204f, 238f, 156f);
        controller.characterRotation = new Vector3(90f, -4f, -3f);

        // Mark the object as dirty so changes are saved
        EditorUtility.SetDirty(controller);

        // If in Play mode, also update CharacterRoot directly
        if (Application.isPlaying)
        {
            if (controller.characterRoot != null)
            {
                controller.characterRoot.position = new Vector3(5f, 0f, 0.5f);
                controller.characterRoot.rotation = Quaternion.identity;
                controller.characterRoot.localScale = Vector3.one;
                Debug.Log($"✓ Updated CharacterRoot position to: {controller.characterRoot.position}");
            }

            // Find and update the character instance
            CharacterEquipmentManager equipMgr = FindFirstObjectByType<CharacterEquipmentManager>();
            if (equipMgr != null)
            {
                equipMgr.transform.localPosition = new Vector3(0f, 0f, 0f);
                equipMgr.transform.localScale = new Vector3(204f, 238f, 156f);
                equipMgr.transform.localRotation = Quaternion.Euler(90f, -4f, -3f);
                Debug.Log($"✓ Updated character transform:");
                Debug.Log($"  Position: {equipMgr.transform.localPosition}");
                Debug.Log($"  Scale: {equipMgr.transform.localScale}");
                Debug.Log($"  Rotation: {equipMgr.transform.localRotation.eulerAngles}");
            }
        }
        else
        {
            // Mark scene as dirty if not in play mode
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            // Automatically save the scene
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("✓ Scene saved with new character transform values!");
        }

        Debug.Log("✓ Character Transform Values Set!");
        Debug.Log($"  characterPosition: {controller.characterPosition}");
        Debug.Log($"  characterScale: {controller.characterScale}");
        Debug.Log($"  characterRotation: {controller.characterRotation}");
        Debug.Log("=== Done ===");
    }
}
