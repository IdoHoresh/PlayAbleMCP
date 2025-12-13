using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class DiagnoseUIIssue : EditorWindow
{
    [MenuItem("Tools/Diagnose UI Layout")]
    public static void Diagnose()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== UI LAYOUT DIAGNOSTIC ===");

        // Find all Canvases
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log($"Found {canvases.Length} Canvas(es):");
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"  - {canvas.name}: RenderMode={canvas.renderMode}, SortOrder={canvas.sortingOrder}");
        }

        // Find all Cameras
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        Debug.Log($"\nFound {cameras.Length} Camera(s):");
        foreach (Camera cam in cameras)
        {
            Debug.Log($"  - {cam.name}: Depth={cam.depth}, TargetTexture={cam.targetTexture != null}, Enabled={cam.enabled}");
            if (cam.targetTexture != null)
            {
                Debug.Log($"    TargetTexture: {cam.targetTexture.name} ({cam.targetTexture.width}x{cam.targetTexture.height})");
            }
        }

        // Find CharacterDisplay
        RawImage[] rawImages = FindObjectsByType<RawImage>(FindObjectsSortMode.None);
        Debug.Log($"\nFound {rawImages.Length} RawImage(s):");
        foreach (RawImage img in rawImages)
        {
            Debug.Log($"  - {img.name}: Texture={img.texture != null}, Active={img.gameObject.activeSelf}");
            RectTransform rect = img.GetComponent<RectTransform>();
            if (rect != null)
            {
                Debug.Log($"    AnchorMin={rect.anchorMin}, AnchorMax={rect.anchorMax}");
                Debug.Log($"    Position={rect.anchoredPosition}, SizeDelta={rect.sizeDelta}");
                Debug.Log($"    SiblingIndex={rect.GetSiblingIndex()}");
            }
        }

        // Check CharacterViewController
        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC != null)
        {
            Debug.Log($"\nCharacterViewController:");
            Debug.Log($"  - characterCamera: {charVC.characterCamera != null}");
            Debug.Log($"  - characterRenderTexture: {charVC.characterRenderTexture != null}");
            Debug.Log($"  - characterDisplayUI: {charVC.characterDisplayUI != null}");
            if (charVC.characterRoot != null)
            {
                Debug.Log($"  - characterRoot position: {charVC.characterRoot.position}");
            }
        }

        Debug.Log("=== END DIAGNOSTIC ===");
    }
}
