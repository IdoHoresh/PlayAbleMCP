using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the 3D character viewport display
/// Creates a dedicated camera rendering to a RenderTexture shown in UI
/// </summary>
public class CharacterViewController : MonoBehaviour
{
    [Header("Character References")]
    public GameObject characterPrefab;
    public Transform characterRoot;
    
    [Header("Camera Settings")]
    public Camera characterCamera;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.2f;
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
        SetupCharacterViewport();
    }

    private void Start()
    {
        // First, try to find existing character in the scene
        FindExistingCharacter();

        // Only spawn if no existing character was found
        if (characterInstance == null && characterPrefab != null)
        {
            SpawnCharacter();
        }
    }

    private void SetupCharacterViewport()
    {
        // Create character root if not assigned
        if (characterRoot == null)
        {
            GameObject rootObj = new GameObject("CharacterRoot");
            characterRoot = rootObj.transform;
            characterRoot.position = new Vector3(5f, 0f, 0f); // Position on right side
            characterRoot.SetParent(transform);
        }

        // Setup camera
        SetupCamera();
        
        // Setup render texture
        SetupRenderTexture();
        
        // Setup lighting
        SetupLighting();
        
        Debug.Log("CharacterViewController: Viewport setup complete");
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
        characterCamera.backgroundColor = new Color(0.2f, 0.2f, 0.25f, 1f); // Neutral background
        // Use Default layer (0) since Character layer doesn't exist
        characterCamera.cullingMask = 1 << 0; // Render Default layer
        characterCamera.depth = -1; // Render before main camera
        characterCamera.fieldOfView = 40f; // Narrower FOV for character close-up
        
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
        characterInstance.transform.localPosition = Vector3.zero;
        characterInstance.transform.localRotation = Quaternion.Euler(0, 180f, 0); // Face camera

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
                characterInstance.transform.localPosition = Vector3.zero;
                characterInstance.transform.localRotation = Quaternion.Euler(0, 180f, 0);
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
