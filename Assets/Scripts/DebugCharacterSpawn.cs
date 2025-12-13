using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCharacterSpawn : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame)
        {
            Debug.Log("=== Character Debug Info ===");

            // Find CharacterViewController
            CharacterViewController viewController = FindFirstObjectByType<CharacterViewController>();

            if (viewController == null)
            {
                Debug.LogError("CharacterViewController not found in scene!");
                return;
            }

            Debug.Log($"CharacterViewController found on: {viewController.gameObject.name}");
            Debug.Log($"Character Prefab assigned: {viewController.characterPrefab != null}");

            if (viewController.characterPrefab != null)
            {
                Debug.Log($"Character Prefab name: {viewController.characterPrefab.name}");
            }

            // Find all CharacterEquipmentManager instances
            CharacterEquipmentManager[] managers = FindObjectsByType<CharacterEquipmentManager>(FindObjectsSortMode.None);
            Debug.Log($"Found {managers.Length} CharacterEquipmentManager(s) in scene:");

            foreach (CharacterEquipmentManager mgr in managers)
            {
                Debug.Log($"  - {mgr.gameObject.name}");
                Debug.Log($"    Position: {mgr.transform.position}");
                Debug.Log($"    LocalScale: {mgr.transform.localScale}");
                Debug.Log($"    Parent: {(mgr.transform.parent != null ? mgr.transform.parent.name : "None")}");
            }

            // Check CharacterRoot
            Transform characterRoot = null;
            foreach (Transform child in viewController.transform)
            {
                if (child.name == "CharacterRoot")
                {
                    characterRoot = child;
                    break;
                }
            }

            if (characterRoot != null)
            {
                Debug.Log($"CharacterRoot found!");
                Debug.Log($"  Children count: {characterRoot.childCount}");

                for (int i = 0; i < characterRoot.childCount; i++)
                {
                    Transform child = characterRoot.GetChild(i);
                    Debug.Log($"  Child {i}: {child.name}, Active: {child.gameObject.activeSelf}");
                }
            }
            else
            {
                Debug.LogWarning("CharacterRoot not found!");
            }
        }
    }
}
