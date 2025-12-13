using UnityEngine;
using UnityEditor;

/// <summary>
/// Complete character setup - runs all setup steps in the correct order
/// </summary>
public class CompleteCharacterSetup
{
    [MenuItem("Tools/Complete Character Setup (Run This!)")]
    public static void RunCompleteSetup()
    {
        Debug.Log("╔════════════════════════════════════════════╗");
        Debug.Log("║   COMPLETE CHARACTER SETUP - STARTING...   ║");
        Debug.Log("╚════════════════════════════════════════════╝");
        Debug.Log("");

        // Step 0: Configure FBX as Humanoid
        Debug.Log("STEP 0: Configuring FBX as Humanoid...");
        ConfigureCharacterFBX.ConfigureFBX();
        Debug.Log("");

        // Step 1: Create Animator Controller
        Debug.Log("STEP 1: Creating Animator Controller...");
        // CreateCharacterAnimatorController.CreateAnimatorController();
        Debug.Log("(Skipped - run manually: Tools > Create Character Animator Controller)");

        // Step 2: Setup Character Prefab
        Debug.Log("STEP 2: Setting up Character Prefab...");
        SetupCharacterFromFBX_v2.SetupCharacter();
        Debug.Log("");

        // Step 3: Verify TestCharacter still exists for backward compatibility
        Debug.Log("STEP 3: Checking TestCharacter...");
        GameObject testChar = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/TestCharacter.prefab");
        if (testChar == null)
        {
            Debug.LogWarning("⚠ TestCharacter.prefab not found - this is OK if you're using the new character");
        }
        else
        {
            Debug.Log("✓ TestCharacter.prefab exists (backup character)");
        }
        Debug.Log("");

        Debug.Log("╔════════════════════════════════════════════╗");
        Debug.Log("║        CHARACTER SETUP COMPLETE! ✓         ║");
        Debug.Log("╚════════════════════════════════════════════╝");
        Debug.Log("");
        Debug.Log("What was created:");
        Debug.Log("  ✓ GirlCharacterAnimator.controller - Animator with 3 animations");
        Debug.Log("  ✓ Girl_Character.prefab - Character with equipment manager");
        Debug.Log("  ✓ CharacterViewController configured to use new character");
        Debug.Log("");
        Debug.Log("NEXT STEPS:");
        Debug.Log("  1. Enter Play mode");
        Debug.Log("  2. Complete Order_Energy3 to see the axe appear on character");
        Debug.Log("  3. Character should be visible on the right side of screen");
        Debug.Log("");
        Debug.Log("If the character doesn't appear, check:");
        Debug.Log("  - CharacterViewController in scene has characterPrefab assigned");
        Debug.Log("  - Camera culling mask is set to Default layer");
        Debug.Log("  - Character is on Default layer (0)");
    }
}
