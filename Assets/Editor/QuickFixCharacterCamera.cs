using UnityEngine;
using UnityEditor;

public class QuickFixCharacterCamera : EditorWindow
{
    [MenuItem("Tools/Quick Fix Character Camera")]
    public static void QuickFix()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== Quick Fix Character Camera ===");

        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC == null)
        {
            Debug.LogError("CharacterViewController not found!");
            return;
        }

        if (charVC.characterCamera != null)
        {
            // Make sure camera only renders to texture, not to screen
            if (charVC.characterRenderTexture != null)
            {
                charVC.characterCamera.targetTexture = charVC.characterRenderTexture;
                Debug.Log("✓ Assigned RenderTexture to camera");
            }
            else
            {
                Debug.LogError("RenderTexture is null!");
            }

            // Make sure camera is enabled
            charVC.characterCamera.enabled = true;
            
            // Set camera to render ONLY to texture (not to screen)
            charVC.characterCamera.depth = -10;
            
            Debug.Log($"Camera settings:");
            Debug.Log($"  - Depth: {charVC.characterCamera.depth}");
            Debug.Log($"  - TargetTexture: {charVC.characterCamera.targetTexture != null}");
            Debug.Log($"  - Enabled: {charVC.characterCamera.enabled}");
        }
        else
        {
            Debug.LogError("Character camera is null!");
        }

        // Check if UI display exists and is visible
        if (charVC.characterDisplayUI != null)
        {
            charVC.characterDisplayUI.enabled = true;
            Debug.Log($"✓ Character display UI: {charVC.characterDisplayUI.gameObject.name}");
            Debug.Log($"  - Texture assigned: {charVC.characterDisplayUI.texture != null}");
            Debug.Log($"  - Active: {charVC.characterDisplayUI.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogWarning("Character display UI is null!");
        }

        Debug.Log("=== Fix Complete ===");
        Debug.Log("If the layout is still broken, the CharacterDisplay RawImage might be covering the entire screen.");
    }
}
