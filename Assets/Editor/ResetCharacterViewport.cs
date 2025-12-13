using UnityEngine;
using UnityEditor;

public class ResetCharacterViewport : EditorWindow
{
    [MenuItem("Tools/Reset Character Viewport")]
    public static void Reset()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Please stop Play mode first!");
            return;
        }

        Debug.Log("=== Resetting Character Viewport ===");

        // Find CharacterViewController
        CharacterViewController controller = FindFirstObjectByType<CharacterViewController>();
        
        if (controller != null)
        {
            // Find and delete old CharacterRoot if it exists
            Transform oldRoot = controller.transform.Find("CharacterRoot");
            if (oldRoot != null)
            {
                Debug.Log("Deleting old CharacterRoot...");
                DestroyImmediate(oldRoot.gameObject);
            }

            // Find and delete old CharacterCamera if it exists
            Transform oldCamera = controller.transform.Find("CharacterCamera");
            if (oldCamera != null)
            {
                Debug.Log("Deleting old CharacterCamera...");
                DestroyImmediate(oldCamera.gameObject);
            }

            // Reset references
            controller.characterRoot = null;
            controller.characterCamera = null;
            controller.characterRenderTexture = null;
            controller.characterDisplayUI = null;

            Debug.Log("✓ Character viewport reset! Press Play to see the updated character.");
        }
        else
        {
            Debug.LogWarning("CharacterViewController not found in scene.");
        }

        Debug.Log("=== Reset Complete ===");
    }
}
