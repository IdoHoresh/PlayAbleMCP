using UnityEngine;
using UnityEditor;

public class ForceUpdateCharacterScale : EditorWindow
{
    [MenuItem("Tools/Force Update Character Scale")]
    public static void ForceUpdate()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please run this while in Play mode!");
            return;
        }

        Debug.Log("=== Force Updating Character Scale ===");

        CharacterViewController charVC = FindFirstObjectByType<CharacterViewController>();
        if (charVC == null)
        {
            Debug.LogError("CharacterViewController not found!");
            return;
        }

        // Get the equipment manager (which is on the character instance)
        CharacterEquipmentManager equipMgr = charVC.GetEquipmentManager();
        if (equipMgr != null)
        {
            GameObject character = equipMgr.gameObject;
            
            Debug.Log($"Found character: {character.name}");
            Debug.Log($"Current scale: {character.transform.localScale}");
            Debug.Log($"Current position: {character.transform.localPosition}");
            Debug.Log($"Parent: {character.transform.parent?.name}");
            
            // Force update to the new scale values
            character.transform.localScale = new Vector3(5f, 5f, 5f);
            character.transform.localPosition = new Vector3(0f, -1f, 0f);
            character.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            
            Debug.Log($"✓ Updated character scale to: {character.transform.localScale}");
            Debug.Log($"✓ Updated character position to: {character.transform.localPosition}");
            
            // Also update the CharacterViewController's public fields so Update() doesn't revert them
            charVC.characterScale = new Vector3(5f, 5f, 5f);
            charVC.characterPosition = new Vector3(0f, -1f, 0f);
            charVC.characterRotation = new Vector3(0f, 180f, 0f);
            
            Debug.Log("✓ Updated CharacterViewController fields");
        }
        else
        {
            Debug.LogError("CharacterEquipmentManager not found!");
        }

        // Check camera framing
        if (charVC.characterCamera != null && charVC.characterRoot != null)
        {
            Debug.Log($"\nCamera info:");
            Debug.Log($"  - Camera position: {charVC.characterCamera.transform.position}");
            Debug.Log($"  - CharacterRoot position: {charVC.characterRoot.position}");
            Debug.Log($"  - Camera distance: {charVC.cameraDistance}");
            Debug.Log($"  - Camera height: {charVC.cameraHeight}");
            Debug.Log($"  - FOV: {charVC.characterCamera.fieldOfView}");
        }

        Debug.Log("=== Update Complete! ===");
        Debug.Log("The character should now be MUCH larger and visible in the right viewport!");
    }
}
