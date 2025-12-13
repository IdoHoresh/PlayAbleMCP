using UnityEngine;
using UnityEditor;

/// <summary>
/// Tool to configure the character FBX import settings to Humanoid
/// </summary>
public class ConfigureCharacterFBX
{
    [MenuItem("Tools/Configure Character FBX as Humanoid")]
    public static void ConfigureFBX()
    {
        Debug.Log("=== Configuring Character FBX ===");

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

        // Set to Humanoid
        if (importer.animationType != ModelImporterAnimationType.Human)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            Debug.Log("✓ Set animation type to Humanoid");
        }
        else
        {
            Debug.Log("✓ Already set to Humanoid");
        }

        // Set avatar creation mode to create from model
        if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
        {
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            Debug.Log("✓ Set avatar setup to CreateFromThisModel");
        }

        // Save and reimport
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log("=== FBX Configuration Complete! ===");
        Debug.Log("The FBX has been reimported as Humanoid.");
        Debug.Log("Now run: Tools > Complete Character Setup (Run This!)");
    }
}
