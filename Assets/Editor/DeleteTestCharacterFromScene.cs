using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DeleteTestCharacterFromScene
{
    [MenuItem("Tools/Delete TestCharacter from Current Scene")]
    public static void DeleteTestCharacter()
    {
        Debug.Log("=== Searching for TestCharacter in current scene ===");
        
        // Find all GameObjects in the scene (including inactive ones)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        
        Debug.Log($"Checking {allObjects.Length} objects in scene...");
        
        bool found = false;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TestCharacter") || obj.name.Contains("Test"))
            {
                Debug.Log($"Found object: {obj.name}");
                
                // Check if it has CharacterEquipmentManager
                CharacterEquipmentManager equipment = obj.GetComponent<CharacterEquipmentManager>();
                if (equipment != null)
                {
                    Debug.Log($"  - Has CharacterEquipmentManager, deleting...");
                    Undo.DestroyObjectImmediate(obj);
                    found = true;
                    Debug.Log($"✓ Deleted: {obj.name}");
                }
            }
        }
        
        if (!found)
        {
            Debug.LogWarning("TestCharacter not found in scene. Checking all root objects:");
            
            // List all root GameObjects
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            Debug.Log($"Root objects in scene: {rootObjects.Length}");
            
            foreach (GameObject root in rootObjects)
            {
                Debug.Log($"  - {root.name}");
                
                // Check children
                CharacterEquipmentManager[] managers = root.GetComponentsInChildren<CharacterEquipmentManager>(true);
                if (managers.Length > 0)
                {
                    Debug.Log($"    Found {managers.Length} CharacterEquipmentManager(s) in children:");
                    foreach (CharacterEquipmentManager mgr in managers)
                    {
                        Debug.Log($"      - {mgr.gameObject.name}");
                        if (mgr.gameObject.name.Contains("Test"))
                        {
                            Debug.Log($"      Deleting: {mgr.gameObject.name}");
                            Undo.DestroyObjectImmediate(mgr.gameObject);
                            found = true;
                        }
                    }
                }
            }
        }
        
        if (found)
        {
            Debug.Log("✓✓✓ TestCharacter removed from scene!");
            Debug.Log("Now checking for CharacterViewController...");
            
            // Find CharacterViewController
            CharacterViewController viewController = GameObject.FindFirstObjectByType<CharacterViewController>();
            if (viewController != null)
            {
                Debug.Log($"Found CharacterViewController on: {viewController.gameObject.name}");
                
                // Check if it has a character prefab assigned
                SerializedObject so = new SerializedObject(viewController);
                SerializedProperty prefabProp = so.FindProperty("characterPrefab");
                
                if (prefabProp.objectReferenceValue == null)
                {
                    Debug.Log("  CharacterViewController has no characterPrefab assigned!");
                    Debug.Log("  Loading Girl_Character prefab...");
                    
                    GameObject girlPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Girl_Character.prefab");
                    if (girlPrefab != null)
                    {
                        prefabProp.objectReferenceValue = girlPrefab;
                        so.ApplyModifiedProperties();
                        Debug.Log("✓ Assigned Girl_Character prefab to CharacterViewController!");
                    }
                    else
                    {
                        Debug.LogError("Girl_Character prefab not found!");
                    }
                }
                else
                {
                    Debug.Log($"  characterPrefab is already set to: {prefabProp.objectReferenceValue.name}");
                }
            }
            else
            {
                Debug.LogWarning("CharacterViewController not found in scene!");
            }
            
            // Mark scene as dirty
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("\n✓ Scene marked as dirty. Don't forget to save!");
        }
        else
        {
            Debug.LogWarning("No TestCharacter found to delete.");
        }
    }
}
