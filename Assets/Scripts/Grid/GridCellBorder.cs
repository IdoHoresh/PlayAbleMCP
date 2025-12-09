using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridCellBorder : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    [Header("Border Settings")]
    public Color borderColor = new Color(1f, 1f, 1f, 0.5f);
    public float borderWidth = 0.02f;
    public float cellSize = 1f;

    private void Awake()
    {
        CreateBorder();
    }

    private void CreateBorder()
    {
        // Add LineRenderer for border
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        // Configure LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = borderColor;
        lineRenderer.endColor = borderColor;
        lineRenderer.startWidth = borderWidth;
        lineRenderer.endWidth = borderWidth;
        lineRenderer.positionCount = 5;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.sortingOrder = 1;

        // Create square border points
        float halfSize = cellSize / 2f;
        Vector3[] positions = new Vector3[5]
        {
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(halfSize, -halfSize, 0),
            new Vector3(halfSize, halfSize, 0),
            new Vector3(-halfSize, halfSize, 0),
            new Vector3(-halfSize, -halfSize, 0)
        };

        lineRenderer.SetPositions(positions);
    }

    public void SetBorderColor(Color color)
    {
        borderColor = color;
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}
