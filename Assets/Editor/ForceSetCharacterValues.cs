using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ForceSetCharacterValues : EditorWindow
{
    [MenuItem("Tools/FORCE Set Character Values (Save Scene)")]
    public static void ForceSet()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Please EXIT Play mode first, then run this tool!");
            return;
        }

        Debug.Log("=== FORCE Setting Character Values ===");

        // Find CharacterViewController
        CharacterViewController controller = FindFirstObjectByType<CharacterViewController>();
        
        if (controller == null)
        {
            Debug.LogError("CharacterViewController not found! Make sure you have a GameManager with CharacterViewController component in the scene.");
            return;
        }

        Debug.Log($"Found CharacterViewController on: {controller.gameObject.name}");

        // Force set the values using SerializedObject (this ensures they're saved to scene)
        SerializedObject serializedController = new SerializedObject(controller);
        
        SerializedProperty positionProp = serializedController.FindProperty("characterPosition");
        SerializedProperty scaleProp = serializedController.FindProperty("characterScale");
        SerializedProperty rotationProp = serializedController.FindProperty("characterRotation");

        positionProp.vector3Value = new Vector3(0f, 0f, 0f);
        scaleProp.vector3Value = new Vector3(204f, 238f, 156f);
        rotationProp.vector3Value = new Vector3(90f, -4f, -3f);

        // Apply changes
        serializedController.ApplyModifiedProperties();

        Debug.Log("✓ Values set via SerializedObject:");
        Debug.Log($"  Position: {positionProp.vector3Value}");
        Debug.Log($"  Scale: {scaleProp.vector3Value}");
        Debug.Log($"  Rotation: {rotationProp.vector3Value}");

        // Mark objects and scene as dirty
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(controller.gameObject);
        
        Scene activeScene = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);

        // FORCE save the scene
        bool saved = EditorSceneManager.SaveScene(activeScene);
        
        if (saved)
        {
            Debug.Log($"✓✓✓ Scene '{activeScene.name}' SAVED successfully!");
            Debug.Log("Now press Play - the character should appear with correct transform!");
        }
        else
        {
            Debug.LogError("Failed to save scene!");
        }

        Debug.Log("=== Done ===");
    }
}
