using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GradientBackground : MonoBehaviour
{
    [Header("Gradient Colors")]
    public Color topColor = new Color(1f, 0.7f, 0.4f); // Light orange
    public Color bottomColor = new Color(1f, 0.5f, 0.2f); // Darker orange

    private SpriteRenderer spriteRenderer;
    private Texture2D gradientTexture;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateGradientSprite();
    }

    private void CreateGradientSprite()
    {
        // Create a texture for the gradient
        int height = 256;
        gradientTexture = new Texture2D(1, height);
        
        // Fill the texture with gradient colors
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color color = Color.Lerp(bottomColor, topColor, t);
            gradientTexture.SetPixel(0, y, color);
        }
        
        gradientTexture.Apply();
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        gradientTexture.filterMode = FilterMode.Bilinear;

        // Create sprite from texture
        Sprite gradientSprite = Sprite.Create(
            gradientTexture,
            new Rect(0, 0, 1, height),
            new Vector2(0.5f, 0.5f),
            1f
        );

        spriteRenderer.sprite = gradientSprite;
        
        // Scale to fill camera view
        ScaleToFillCamera();
    }

    private void ScaleToFillCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        transform.localScale = new Vector3(width, height, 1f);
        
        // Position at camera center
        Vector3 camPos = cam.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y, 0f);
    }

    private void OnValidate()
    {
        if (Application.isPlaying && spriteRenderer != null)
        {
            CreateGradientSprite();
        }
    }
}
