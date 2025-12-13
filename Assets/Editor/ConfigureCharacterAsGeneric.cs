using UnityEngine;
using UnityEditor;

/// <summary>
/// Configure the character FBX as Generic instead of Humanoid
/// Use this when the character doesn't have standard humanoid bones
/// </summary>
public class ConfigureCharacterAsGeneric
{
    [MenuItem("Tools/Configure Character as Generic Rig")]
    public static void ConfigureAsGeneric()
    {
        Debug.Log("=== Configuring Character as Generic Rig ===");

        string fbxPath = "Assets/Models/Character/Animated_Girl_in_Casu_1212214333_generate.fbx";

        // Get the model importer
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;

        if (importer == null)
        {
            Debug.LogError($"Could not find FBX at {fbxPath}");
            return;
        }

        Debug.Log($"Found FBX importer for: {fbxPath}");
        Debug.Log($"Current animation type: {importer.animationType}");

        // Set to Generic
        importer.animationType = ModelImporterAnimationType.Generic;
        Debug.Log("✓ Set animation type to Generic");

        // Save and reimport
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log("=== FBX Configuration Complete! ===");
        Debug.Log("The FBX has been reimported as Generic rig.");
        Debug.Log("Now run: Tools > Setup Character (Generic Rig)");
    }
}
