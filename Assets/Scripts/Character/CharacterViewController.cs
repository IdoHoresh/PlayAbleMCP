using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the 3D character viewport display
/// Creates a dedicated camera rendering to a RenderTexture shown in UI
/// </summary>
[ExecuteInEditMode]
public class CharacterViewController : MonoBehaviour
{
    [Header("Character References")]
    public GameObject characterPrefab;
    public Transform characterRoot;

    [Header("Character Transform")]
    public Vector3 characterPosition = new Vector3(0f, 0f, 0f);
    public Vector3 characterScale = new Vector3(204f, 238f, 156f);
    public Vector3 characterRotation = new Vector3(90f, -4f, -3f);

    [Header("Edit Mode")]
    public bool showCharacterInEditMode = true;
    
    [Header("Camera Settings")]
    public Camera characterCamera;
    public float cameraDistance = 2f;
    public float cameraHeight = 0.8f;
    public float cameraAngle = 15f;
    
    [Header("Render Settings")]
    public RenderTexture characterRenderTexture;
    public RawImage characterDisplayUI;
    public int renderTextureWidth = 512;
    public int renderTextureHeight = 1024;
    
    [Header("Lighting")]
    public Light characterLight;
    public Color lightColor = Color.white;
    public float lightIntensity = 1.2f;
    
    private GameObject characterInstance;
    private CharacterAnimationController animController;
    private CharacterEquipmentManager equipmentManager;

    private void Awake()
    {
        // Viewport setup moved to Start() to ensure Canvas exists first
    }

    private void Start()
    {
        // Only run in Play mode
        if (!Application.isPlaying) return;

        // Setup viewport first (Canvas should exist by now since UISetup runs in Awake)
        SetupCharacterViewport();

        // First, try to find existing character in the scene
        FindExistingCharacter();

        // Only spawn if no existing character was found
        if (characterInstance == null && characterPrefab != null)
        {
            SpawnCharacter();
            Debug.Log($"CharacterViewController: Spawned new character at position {characterPosition}, scale {characterScale}");
        }
        else if (characterInstance != null)
        {
            Debug.Log($"CharacterViewController: Using existing character '{characterInstance.name}'");

            // Update transform values for existing character
            if (characterRoot != null)
            {
                // Update CharacterRoot to correct position
                characterRoot.position = new Vector3(5f, 0f, 0.5f);
                Debug.Log($"CharacterViewController: Repositioned CharacterRoot to {characterRoot.position}");

                characterInstance.transform.SetParent(characterRoot);
                characterInstance.transform.localPosition = characterPosition;
                characterInstance.transform.localScale = characterScale;
                characterInstance.transform.localRotation = Quaternion.Euler(characterRotation);
                Debug.Log($"CharacterViewController: Updated existing character transform - scale: {characterScale}, rotation: {characterRotation}");
            }
        }
        else
        {
            Debug.LogError("CharacterViewController: No character prefab assigned!");
        }
    }

    private void Update()
    {
        // In Edit mode, spawn/update character if needed
        if (!Application.isPlaying && showCharacterInEditMode)
        {
            if (characterInstance == null && characterPrefab != null)
            {
                FindExistingCharacter();
                if (characterInstance == null)
                {
                    SpawnCharacter();
                }
            }
            else if (characterInstance != null)
            {
                // Update transform in real-time
                characterInstance.transform.localPosition = characterPosition;
                characterInstance.transform.localScale = characterScale;
                characterInstance.transform.localRotation = Quaternion.Euler(characterRotation);
            }
        }

        // In Play mode, FORCE update character transform every frame
        if (Application.isPlaying)
        {
            // Find CharacterRoot if we don't have a reference
            if (characterRoot == null)
            {
                Transform found = transform.Find("CharacterRoot");
                if (found != null)
                {
                    characterRoot = found;
                }
            }

            // Always keep CharacterRoot at correct position
            if (characterRoot != null)
            {
                characterRoot.position = new Vector3(5f, 0f, 0.5f);
                characterRoot.rotation = Quaternion.identity;
                characterRoot.localScale = Vector3.one;
            }

            // Try to find character if we don't have a reference
            if (characterInstance == null)
            {
                CharacterEquipmentManager equipMgr = FindFirstObjectByType<CharacterEquipmentManager>();
                if (equipMgr != null)
                {
                    characterInstance = equipMgr.gameObject;
                }
            }

            // Update character transform
            if (characterInstance != null)
            {
                characterInstance.transform.localPosition = characterPosition;
                characterInstance.transform.localScale = characterScale;
                characterInstance.transform.localRotation = Quaternion.Euler(characterRotation);
            }
        }
    }

    private void SetupCharacterViewport()
    {
        // Create character root if not assigned
        if (characterRoot == null)
        {
            GameObject rootObj = new GameObject("CharacterRoot");
            characterRoot = rootObj.transform;
            characterRoot.SetParent(transform);
        }

        // Always update CharacterRoot position to correct values
        characterRoot.position = new Vector3(5f, 0f, 0.5f);
        characterRoot.rotation = Quaternion.identity;
        characterRoot.localScale = Vector3.one;

        // NOTE: Using main camera instead of separate camera for simplicity
        // No need for render texture - character renders in world space

        Debug.Log($"CharacterViewController: Viewport setup complete at position {characterRoot.position}");
    }

    private void SetupCamera()
    {
        if (characterCamera == null)
        {
            GameObject camObj = new GameObject("CharacterCamera");
            camObj.transform.SetParent(transform);
            characterCamera = camObj.AddComponent<Camera>();
        }

        // Configure camera for character only
        characterCamera.clearFlags = CameraClearFlags.SolidColor;
        characterCamera.backgroundColor = new Color(0.8f, 0.7f, 0.6f, 1f); // Match background color
        // Use Default layer (0) since Character layer doesn't exist
        characterCamera.cullingMask = 1 << 0; // Render Default layer
        characterCamera.depth = -10; // Render before main camera (won't interfere with UI)
        characterCamera.fieldOfView = 30f; // Narrower FOV for character close-up (tighter framing)

        // IMPORTANT: Don't render to screen, only to RenderTexture
        // The targetTexture will be set in SetupRenderTexture()
        
        // Position camera to look at character
        PositionCamera();
    }

    private void PositionCamera()
    {
        if (characterCamera == null || characterRoot == null) return;

        // Position camera at an angle facing the character
        Vector3 offset = new Vector3(0f, cameraHeight, cameraDistance);
        characterCamera.transform.position = characterRoot.position + offset;
        characterCamera.transform.LookAt(characterRoot.position + Vector3.up * cameraHeight);
        characterCamera.transform.RotateAround(characterRoot.position, Vector3.up, -25f); // Slight side angle
    }

    private void SetupRenderTexture()
    {
        // Create render texture if not assigned
        if (characterRenderTexture == null)
        {
            characterRenderTexture = new RenderTexture(
                renderTextureWidth, 
                renderTextureHeight, 
                24, // Depth buffer bits
                RenderTextureFormat.ARGB32
            );
            characterRenderTexture.name = "CharacterRT";
            characterRenderTexture.antiAliasing = 1; // No AA for mobile performance
        }

        // Assign to camera
        if (characterCamera != null)
        {
            characterCamera.targetTexture = characterRenderTexture;
        }

        // Create UI display if not exists
        if (characterDisplayUI == null)
        {
            CreateCharacterUIDisplay();
        }
        else
        {
            characterDisplayUI.texture = characterRenderTexture;
        }
    }

    private void CreateCharacterUIDisplay()
    {
        // Find the canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("CharacterViewController: No Canvas found!");
            return;
        }

        // Create RawImage for character display
        GameObject displayObj = new GameObject("CharacterDisplay");
        displayObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = displayObj.AddComponent<RectTransform>();
        
        // Anchor to right side of screen (0.5 to 1.0 in X)
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(0, 0); // Left and bottom
        rect.offsetMax = new Vector2(0, 0); // Right and top

        characterDisplayUI = displayObj.AddComponent<RawImage>();
        characterDisplayUI.texture = characterRenderTexture;
        
        // Move behind UI elements
        displayObj.transform.SetAsFirstSibling();

        Debug.Log("CharacterViewController: UI display created on right side");
    }

    private void SetupLighting()
    {
        if (characterLight == null)
        {
            GameObject lightObj = new GameObject("CharacterLight");
            lightObj.transform.SetParent(characterRoot);
            characterLight = lightObj.AddComponent<Light>();
        }

        characterLight.type = LightType.Directional;
        characterLight.color = lightColor;
        characterLight.intensity = lightIntensity;
        characterLight.shadows = LightShadows.None; // Disable shadows for performance
        characterLight.cullingMask = 1 << 0; // Light Default layer
        
        // Position light at an angle
        characterLight.transform.position = characterRoot.position + new Vector3(2f, 3f, 2f);
        characterLight.transform.LookAt(characterRoot.position);
    }

    /// <summary>
    /// Instantiate the character model in the viewport
    /// Call this after importing your 3D character from Mixamo
    /// </summary>
    public void SpawnCharacter()
    {
        if (characterPrefab == null)
        {
            Debug.LogWarning("CharacterViewController: No character prefab assigned yet");
            return;
        }

        if (characterInstance != null)
        {
            Destroy(characterInstance);
        }

        characterInstance = Instantiate(characterPrefab, characterRoot);
        characterInstance.transform.localPosition = characterPosition;
        characterInstance.transform.localScale = characterScale;
        // Adjust rotation to face camera (accounting for FBX rotation)
        characterInstance.transform.localRotation = Quaternion.Euler(characterRotation);

        // Keep on Default layer (layer 0) since Character layer doesn't exist
        // SetLayerRecursively(characterInstance, LayerMask.NameToLayer("Character"));

        // Get or add components
        animController = characterInstance.GetComponent<CharacterAnimationController>();
        if (animController == null)
        {
            animController = characterInstance.AddComponent<CharacterAnimationController>();
        }

        equipmentManager = characterInstance.GetComponent<CharacterEquipmentManager>();
        if (equipmentManager == null)
        {
            equipmentManager = characterInstance.AddComponent<CharacterEquipmentManager>();
        }

        Debug.Log("CharacterViewController: Character spawned successfully");
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public CharacterAnimationController GetAnimationController()
    {
        return animController;
    }

    public CharacterEquipmentManager GetEquipmentManager()
    {
        return equipmentManager;
    }

    public Vector3 GetCharacterWorldPosition()
    {
        return characterRoot != null ? characterRoot.position : Vector3.zero;
    }

    /// <summary>
    /// Find an existing character in the scene that already has equipment/animation managers
    /// </summary>
    private void FindExistingCharacter()
    {
        // Look for CharacterEquipmentManager in the scene
        CharacterEquipmentManager foundEquipment = FindFirstObjectByType<CharacterEquipmentManager>();
        if (foundEquipment != null)
        {
            characterInstance = foundEquipment.gameObject;
            equipmentManager = foundEquipment;
            animController = characterInstance.GetComponent<CharacterAnimationController>();

            // Move character to character root
            if (characterRoot != null)
            {
                characterInstance.transform.SetParent(characterRoot);
                characterInstance.transform.localPosition = characterPosition;
                characterInstance.transform.localScale = characterScale;
                // Adjust rotation to face camera (accounting for FBX rotation)
                characterInstance.transform.localRotation = Quaternion.Euler(characterRotation);
            }

            // Keep on Default layer (layer 0) since Character layer doesn't exist
            // SetLayerRecursively(characterInstance, LayerMask.NameToLayer("Character"));

            Debug.Log($"CharacterViewController: Found existing character '{characterInstance.name}' with equipment manager");
        }
        else
        {
            Debug.LogWarning("CharacterViewController: No character prefab assigned and no existing character found in scene");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && characterCamera != null)
        {
            PositionCamera();
        }
    }
#endif
}
