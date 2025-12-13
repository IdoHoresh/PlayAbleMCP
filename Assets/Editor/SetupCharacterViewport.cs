using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupCharacterViewport : EditorWindow
{
    [MenuItem("Tools/Setup Character Viewport")]
    public static void Setup()
    {
        Debug.Log("=== Setting up Character Viewport ===");

        // Make sure we're in the correct scene
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "SampleScene")
        {
            Debug.LogWarning("Please open SampleScene first!");
            return;
        }

        // Find or create CharacterViewController GameObject
        CharacterViewController existingController = FindFirstObjectByType<CharacterViewController>();
        GameObject controllerObj;

        if (existingController != null)
        {
            Debug.Log("Found existing CharacterViewController");
            controllerObj = existingController.gameObject;
        }
        else
        {
            Debug.Log("Creating new CharacterViewController GameObject");
            controllerObj = new GameObject("CharacterViewController");
            existingController = controllerObj.AddComponent<CharacterViewController>();
        }

        // Load the Girl_Character prefab
        GameObject girlPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Girl_Character.prefab");
        
        if (girlPrefab == null)
        {
            Debug.LogError("Girl_Character prefab not found at Assets/Models/Character/Girl_Character.prefab");
            return;
        }

        // Assign the prefab
        existingController.characterPrefab = girlPrefab;
        Debug.Log($"✓ Assigned Girl_Character prefab to CharacterViewController");

        // Verify transform values are correct
        existingController.characterPosition = new Vector3(0f, 0f, 0f);
        existingController.characterScale = new Vector3(1f, 1f, 1f);
        existingController.characterRotation = new Vector3(0f, 180f, 0f);
        Debug.Log($"✓ Set character transform values");

        // Find UISetup and link the CharacterViewController
        UISetup uiSetup = FindFirstObjectByType<UISetup>();
        if (uiSetup != null)
        {
            uiSetup.characterViewController = existingController;
            Debug.Log($"✓ Linked CharacterViewController to UISetup");
        }
        else
        {
            Debug.LogWarning("UISetup not found in scene. You may need to assign CharacterViewController manually.");
        }

        // Mark scene as dirty so changes are saved
        EditorSceneManager.MarkSceneDirty(activeScene);

        Debug.Log("=== Character Viewport Setup Complete ===");
        Debug.Log("Press Play to see the character on the right side of the screen!");
    }
}
