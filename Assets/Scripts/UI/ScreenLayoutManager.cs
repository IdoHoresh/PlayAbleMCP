using UnityEngine;

/// <summary>
/// Manages the split-screen layout for the playable ad
/// Left side: Merge grid
/// Right side: 3D character viewport
/// </summary>
public class ScreenLayoutManager : MonoBehaviour
{
    [Header("Layout Settings")]
    [Range(0.3f, 0.7f)]
    public float gridWidthPercentage = 0.5f; // 50% for grid, 50% for character

    [Header("Viewport Boundaries")]
    public Rect gridViewport;
    public Rect characterViewport;

    private void Awake()
    {
        CalculateViewports();
    }

    private void CalculateViewports()
    {
        // Grid viewport - Left side
        gridViewport = new Rect(
            0f,                      // x: Start at left edge
            0f,                      // y: Start at bottom
            gridWidthPercentage,     // width: Percentage of screen
            1f                       // height: Full height
        );

        // Character viewport - Right side
        characterViewport = new Rect(
            gridWidthPercentage,           // x: Start after grid
            0f,                            // y: Start at bottom
            1f - gridWidthPercentage,      // width: Remaining percentage
            1f                             // height: Full height
        );

        Debug.Log($"ScreenLayout: Grid viewport = {gridViewport}, Character viewport = {characterViewport}");
    }

    /// <summary>
    /// Get the world position for the grid center based on layout
    /// </summary>
    public Vector2 GetGridCenterWorldPosition()
    {
        // Convert viewport center to world position
        float viewportCenterX = gridWidthPercentage / 2f;
        Camera mainCam = Camera.main;
        
        if (mainCam != null)
        {
            Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(viewportCenterX, 0.5f, 10f));
            return new Vector2(worldPos.x, worldPos.y);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Get the screen space position for character viewport center
    /// </summary>
    public Vector2 GetCharacterViewportCenter()
    {
        float centerX = gridWidthPercentage + (1f - gridWidthPercentage) / 2f;
        return new Vector2(centerX, 0.5f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            CalculateViewports();

        // Visualize viewports in scene view
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // Draw grid viewport
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        DrawViewportGizmo(mainCam, gridViewport);

        // Draw character viewport
        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        DrawViewportGizmo(mainCam, characterViewport);
    }

    private void DrawViewportGizmo(Camera cam, Rect viewport)
    {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(viewport.x, viewport.y, 10f));
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(viewport.x, viewport.y + viewport.height, 10f));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(viewport.x + viewport.width, viewport.y + viewport.height, 10f));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(viewport.x + viewport.width, viewport.y, 10f));

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
#endif
}
