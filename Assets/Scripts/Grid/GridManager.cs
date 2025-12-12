using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 3;
    public int gridHeight = 3;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    [Header("Grid Offset")]
    public Vector2 gridOffset = new Vector2(-2.5f, -0.5f); // Positioned on left side of screen for split-screen layout

    private GridCell[,] grid;

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = GridToWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";

                GridCell cell = cellObj.GetComponent<GridCell>();
                if (cell == null)
                {
                    cell = cellObj.AddComponent<GridCell>();
                }
                
                cell.gridPosition = new Vector2Int(x, y);
                grid[x, y] = cell;
            }
        }
    }

    public Vector2 GridToWorldPosition(int x, int y)
    {
        // Center the grid around the grid offset point
        float startX = gridOffset.x - (gridWidth * cellSize / 2f) + (cellSize / 2f);
        float startY = gridOffset.y - (gridHeight * cellSize / 2f) + (cellSize / 2f);
        
        return new Vector2(startX + x * cellSize, startY + y * cellSize);
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPos)
    {
        float startX = gridOffset.x - (gridWidth * cellSize / 2f);
        float startY = gridOffset.y - (gridHeight * cellSize / 2f);

        int x = Mathf.FloorToInt((worldPos.x - startX) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - startY) / cellSize);

        return new Vector2Int(x, y);
    }

    public bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    public GridCell GetCell(Vector2Int pos)
    {
        if (IsValidGridPosition(pos))
        {
            return grid[pos.x, pos.y];
        }
        return null;
    }

    public GridCell GetCell(int x, int y)
    {
        return GetCell(new Vector2Int(x, y));
    }

    public bool IsCellOccupied(Vector2Int pos)
    {
        GridCell cell = GetCell(pos);
        return cell != null && cell.IsOccupied;
    }

    public void PlaceItem(MergeableItem item, Vector2Int gridPos)
    {
        GridCell cell = GetCell(gridPos);
        if (cell != null)
        {
            cell.SetItem(item);
            item.transform.position = GridToWorldPosition(gridPos.x, gridPos.y);
        }
    }

    public void RemoveItem(Vector2Int gridPos)
    {
        GridCell cell = GetCell(gridPos);
        if (cell != null)
        {
            cell.ClearItem();
        }
    }

    public void HighlightCell(Vector2Int pos, bool valid)
    {
        GridCell cell = GetCell(pos);
        if (cell != null)
        {
            cell.Highlight(valid);
        }
    }

    public void ResetAllCellVisuals()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].ResetVisual();
            }
        }
    }
}