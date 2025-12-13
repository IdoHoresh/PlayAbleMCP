using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixUILayout : EditorWindow
{
    [MenuItem("Tools/Fix UI Layout Now")]
    public static void Fix()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== Fixing UI Layout ===");

        // Find CharacterViewController
        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC == null)
        {
            Debug.LogError("CharacterViewController not found!");
            return;
        }

        // Find the CharacterDisplay RawImage
        RawImage characterDisplay = charVC.characterDisplayUI;
        if (characterDisplay != null)
        {
            Debug.Log($"Found CharacterDisplay: {characterDisplay.gameObject.name}");
            
            RectTransform rect = characterDisplay.GetComponent<RectTransform>();
            
            // Set to right half of screen
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchoredPosition = Vector2.zero;
            
            // Move to back of UI (renders first, behind other UI)
            rect.SetAsFirstSibling();
            
            // Make sure it has the texture
            characterDisplay.texture = charVC.characterRenderTexture;
            characterDisplay.color = Color.white;
            characterDisplay.enabled = true;
            
            Debug.Log($"✓ Fixed CharacterDisplay:");
            Debug.Log($"  - AnchorMin: {rect.anchorMin}, AnchorMax: {rect.anchorMax}");
            Debug.Log($"  - Position: {rect.anchoredPosition}");
            Debug.Log($"  - Sibling index: {rect.GetSiblingIndex()}");
        }
        else
        {
            Debug.LogWarning("CharacterDisplay RawImage not found!");
        }

        // Verify camera settings
        if (charVC.characterCamera != null)
        {
            charVC.characterCamera.targetTexture = charVC.characterRenderTexture;
            charVC.characterCamera.enabled = true;
            Debug.Log($"✓ Camera: TargetTexture assigned, Enabled={charVC.characterCamera.enabled}");
        }

        // Find and verify main UI elements
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✓ Canvas: {canvas.name}, RenderMode={canvas.renderMode}");
            
            // List all top-level children
            Debug.Log($"Canvas children:");
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);
                Debug.Log($"  {i}: {child.name}");
            }
        }

        Debug.Log("=== Fix Complete! ===");
        Debug.Log("The character should now be visible on the RIGHT half of the screen");
        Debug.Log("The grid and UI should be on the LEFT half");
    }
}
