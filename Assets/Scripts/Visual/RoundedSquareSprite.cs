using UnityEngine;

/// <summary>
/// Helper script to generate a rounded square sprite programmatically
/// Attach this to a GameObject with a SpriteRenderer to create rounded squares
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RoundedSquareSprite : MonoBehaviour
{
    [Header("Sprite Settings")]
    [Range(32, 512)]
    public int textureSize = 128;

    [Range(0f, 0.5f)]
    public float cornerRadius = 0.2f;

    public Color fillColor = Color.white;

    [Header("Auto Create")]
    public bool createOnStart = true;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (createOnStart)
        {
            CreateRoundedSquare();
        }
    }

    [ContextMenu("Create Rounded Square")]
    public void CreateRoundedSquare()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Bilinear;

        float radius = textureSize * cornerRadius;

        // Fill texture with rounded square
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                bool isInside = IsInsideRoundedSquare(x, y, textureSize, radius);
                Color pixelColor = isInside ? fillColor : Color.clear;
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();

        // Create sprite from texture
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize
        );

        spriteRenderer.sprite = sprite;
    }

    private bool IsInsideRoundedSquare(int x, int y, int size, float radius)
    {
        float halfSize = size / 2f;
        float dx = Mathf.Abs(x - halfSize);
        float dy = Mathf.Abs(y - halfSize);

        // Inside the main square
        if (dx <= halfSize - radius && dy <= halfSize - radius)
            return true;

        // Check corners
        if (dx > halfSize - radius && dy > halfSize - radius)
        {
            float cornerDist = Mathf.Sqrt(
                Mathf.Pow(dx - (halfSize - radius), 2) +
                Mathf.Pow(dy - (halfSize - radius), 2)
            );
            return cornerDist <= radius;
        }

        // Inside rectangle parts
        if (dx <= halfSize - radius || dy <= halfSize - radius)
            return true;

        return false;
    }

    private void OnValidate()
    {
        if (Application.isPlaying && spriteRenderer != null)
        {
            CreateRoundedSquare();
        }
    }
}
