using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor tool to create an Animator Controller for the character
/// Sets up Idle, Receive Item, and Victory animations
/// </summary>
public class CreateCharacterAnimatorController
{
    [MenuItem("Tools/Create Character Animator Controller")]
    public static void CreateAnimatorController()
    {
        Debug.Log("=== Creating Character Animator Controller ===");

        // Create animator controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/GirlCharacterAnimator.controller");

        // Load animation clips
        AnimationClip idleClip = LoadAnimationClip("Assets/Animations/Animation_Idle_6_withSkin.fbx");
        AnimationClip receiveClip = LoadAnimationClip("Assets/Animations/Animation_Excited_Walk_F_without_skin.fbx");
        AnimationClip victoryClip = LoadAnimationClip("Assets/Animations/Animation_victory_without_skin.fbx");

        if (idleClip == null || receiveClip == null || victoryClip == null)
        {
            Debug.LogError("Failed to load one or more animation clips!");
            return;
        }

        Debug.Log($"✓ Loaded animation clips: Idle={idleClip.name}, Receive={receiveClip.name}, Victory={victoryClip.name}");

        // Get the root state machine
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Create states
        AnimatorState idleState = rootStateMachine.AddState("Idle", new Vector3(300, 0, 0));
        idleState.motion = idleClip;

        AnimatorState receiveState = rootStateMachine.AddState("ReceiveItem", new Vector3(300, 100, 0));
        receiveState.motion = receiveClip;

        AnimatorState victoryState = rootStateMachine.AddState("Victory", new Vector3(300, 200, 0));
        victoryState.motion = victoryClip;

        // Set idle as default state
        rootStateMachine.defaultState = idleState;

        // Add parameters
        controller.AddParameter("ReceiveItem", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Celebrate", AnimatorControllerParameterType.Trigger);

        // Create transitions: Idle -> ReceiveItem -> Idle
        AnimatorStateTransition idleToReceive = idleState.AddTransition(receiveState);
        idleToReceive.AddCondition(AnimatorConditionMode.If, 0, "ReceiveItem");
        idleToReceive.hasExitTime = false;
        idleToReceive.duration = 0.2f;

        AnimatorStateTransition receiveToIdle = receiveState.AddTransition(idleState);
        receiveToIdle.hasExitTime = true;
        receiveToIdle.exitTime = 0.9f;
        receiveToIdle.duration = 0.2f;

        // Create transitions: Idle -> Victory -> Idle
        AnimatorStateTransition idleToVictory = idleState.AddTransition(victoryState);
        idleToVictory.AddCondition(AnimatorConditionMode.If, 0, "Celebrate");
        idleToVictory.hasExitTime = false;
        idleToVictory.duration = 0.2f;

        AnimatorStateTransition victoryToIdle = victoryState.AddTransition(idleState);
        victoryToIdle.hasExitTime = true;
        victoryToIdle.exitTime = 0.9f;
        victoryToIdle.duration = 0.2f;

        Debug.Log("✓ Created animator states and transitions");

        // Save
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✓ Animator Controller created at: Assets/Animations/GirlCharacterAnimator.controller");

        // Try to assign to character prefab
        GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Character/Girl_Character.prefab");
        if (characterPrefab != null)
        {
            // Get the Animator component from the prefab
            Animator animator = characterPrefab.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                // Load the prefab for editing
                string prefabPath = AssetDatabase.GetAssetPath(characterPrefab);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                Animator prefabAnimator = prefabRoot.GetComponentInChildren<Animator>();

                if (prefabAnimator != null)
                {
                    prefabAnimator.runtimeAnimatorController = controller;
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                    Debug.Log("✓ Assigned Animator Controller to character prefab");
                }
            }
        }

        Debug.Log("=== Animator Controller Setup Complete! ===");
    }

    private static AnimationClip LoadAnimationClip(string fbxPath)
    {
        // Load all assets from the FBX
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

        // Find the animation clip
        foreach (Object asset in assets)
        {
            if (asset is AnimationClip clip)
            {
                // Skip the "__preview__" clip that Unity creates
                if (!clip.name.Contains("__preview__"))
                {
                    return clip;
                }
            }
        }

        Debug.LogError($"Could not find animation clip in {fbxPath}");
        return null;
    }
}
