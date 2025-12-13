using UnityEngine;
using UnityEditor;

public class CompleteCharacterFix : EditorWindow
{
    [MenuItem("Tools/Complete Character Fix")]
    public static void Fix()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== COMPLETE CHARACTER FIX ===");

        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC == null)
        {
            Debug.LogError("CharacterViewController not found!");
            return;
        }

        // Step 1: Update CharacterRoot position
        if (charVC.characterRoot != null)
        {
            charVC.characterRoot.position = new Vector3(50f, 0f, 0f);
            Debug.Log($"✓ CharacterRoot position: {charVC.characterRoot.position}");
        }

        // Step 2: Update character scale
        CharacterEquipmentManager equipMgr = charVC.GetEquipmentManager();
        if (equipMgr != null)
        {
            GameObject character = equipMgr.gameObject;
            character.transform.localScale = new Vector3(5f, 5f, 5f);
            character.transform.localPosition = new Vector3(0f, -1f, 0f);
            character.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            
            Debug.Log($"✓ Character scale: {character.transform.localScale}");
            Debug.Log($"✓ Character local position: {character.transform.localPosition}");
            Debug.Log($"✓ Character world position: {character.transform.position}");
            
            // Make sure character is active
            character.SetActive(true);
        }

        // Step 3: Fix camera
        if (charVC.characterCamera != null)
        {
            charVC.characterCamera.enabled = true;
            charVC.characterCamera.targetTexture = charVC.characterRenderTexture;
            charVC.characterCamera.cullingMask = 1 << 0; // Layer 0 (Default)
            charVC.characterCamera.clearFlags = CameraClearFlags.SolidColor;
            charVC.characterCamera.backgroundColor = new Color(0.8f, 0.7f, 0.6f, 1f);
            charVC.characterCamera.fieldOfView = 30f;
            
            // Position camera
            if (charVC.characterRoot != null)
            {
                Vector3 characterPos = charVC.characterRoot.position;
                Vector3 cameraPos = characterPos + new Vector3(0f, 0.8f, 2f);
                charVC.characterCamera.transform.position = cameraPos;
                charVC.characterCamera.transform.LookAt(characterPos + Vector3.up * 0.8f);
            }
            
            Debug.Log($"✓ Camera position: {charVC.characterCamera.transform.position}");
            Debug.Log($"✓ Camera target texture: {charVC.characterCamera.targetTexture != null}");
            Debug.Log($"✓ Camera enabled: {charVC.characterCamera.enabled}");
            Debug.Log($"✓ Camera FOV: {charVC.characterCamera.fieldOfView}");
        }

        // Step 4: Verify RenderTexture
        if (charVC.characterRenderTexture != null)
        {
            Debug.Log($"✓ RenderTexture: {charVC.characterRenderTexture.width}x{charVC.characterRenderTexture.height}");
        }
        else
        {
            Debug.LogError("❌ RenderTexture is NULL!");
        }

        // Step 5: Verify UI Display
        if (charVC.characterDisplayUI != null)
        {
            charVC.characterDisplayUI.texture = charVC.characterRenderTexture;
            charVC.characterDisplayUI.enabled = true;
            charVC.characterDisplayUI.color = Color.white;
            Debug.Log($"✓ UI Display texture assigned: {charVC.characterDisplayUI.texture != null}");
        }
        else
        {
            Debug.LogError("❌ CharacterDisplay UI is NULL!");
        }

        // Step 6: Find all objects at CharacterRoot position to see what's there
        Debug.Log("\n=== Objects near CharacterRoot ===");
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            if (Vector3.Distance(obj.transform.position, new Vector3(50f, 0f, 0f)) < 10f)
            {
                Debug.Log($"  - {obj.name} at {obj.transform.position}, layer: {LayerMask.LayerToName(obj.layer)}, active: {obj.activeInHierarchy}");
                count++;
            }
        }
        Debug.Log($"Found {count} objects near CharacterRoot");

        Debug.Log("\n=== FIX COMPLETE ===");
        Debug.Log("If you still don't see the character, the issue might be:");
        Debug.Log("1. Character is not in camera view");
        Debug.Log("2. Character layer doesn't match camera culling mask");
        Debug.Log("3. RenderTexture is not being displayed correctly");
    }
}
