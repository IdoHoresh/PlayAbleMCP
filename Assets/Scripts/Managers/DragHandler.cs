using UnityEngine;
using UnityEngine.InputSystem;

public class DragHandler : MonoBehaviour
{
    private Camera mainCamera;
    private MergeableItem draggedItem;
    private Vector3 dragOffset;
    private Vector2Int originalGridPosition;
    private GridManager gridManager;
    private MergeManager mergeManager;

    [Header("Drag Settings")]
    public float dragZPosition = -1f; // Z position while dragging (in front of grid)
    public LayerMask itemLayerMask;

    private void Awake()
    {
        mainCamera = Camera.main;
        gridManager = FindFirstObjectByType<GridManager>();
        mergeManager = FindFirstObjectByType<MergeManager>();
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPickUpItem();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed && draggedItem != null)
        {
            DragItem();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame && draggedItem != null)
        {
            TryPlaceItem();
        }
    }

    private void TryPickUpItem()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, itemLayerMask);

        if (hit.collider != null)
        {
            MergeableItem item = hit.collider.GetComponent<MergeableItem>();
            if (item != null)
            {
                StartDragging(item);
            }
        }
    }

    private void StartDragging(MergeableItem item)
    {
        draggedItem = item;
        originalGridPosition = item.GetGridPosition();

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = draggedItem.transform.position - mouseWorldPos;

        // Remove from grid temporarily
        gridManager.RemoveItem(originalGridPosition);

        // Apply pickup visual effect
        ItemVisualEffects effects = draggedItem.GetComponent<ItemVisualEffects>();
        if (effects != null)
        {
            effects.OnPickup();
        }

        Debug.Log($"Started dragging {item.itemData.itemName} from {originalGridPosition}");
    }

    private void DragItem()
    {
        Vector3 newPosition = GetMouseWorldPosition() + dragOffset;
        newPosition.z = dragZPosition;
        draggedItem.transform.position = newPosition;

        // Show visual feedback on grid
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2Int targetGridPos = gridManager.WorldToGridPosition(mousePos);

        gridManager.ResetAllCellVisuals();

        if (gridManager.IsValidGridPosition(targetGridPos))
        {
            GridCell targetCell = gridManager.GetCell(targetGridPos);
            MergeableItem targetItem = targetCell.GetItem();

            bool canMerge = targetItem != null && draggedItem.CanMergeWith(targetItem);
            bool canPlace = targetItem == null;

            gridManager.HighlightCell(targetGridPos, canMerge || canPlace);
        }
    }

    private void TryPlaceItem()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2Int targetGridPos = gridManager.WorldToGridPosition(mousePos);

        bool placed = false;

        if (gridManager.IsValidGridPosition(targetGridPos))
        {
            GridCell targetCell = gridManager.GetCell(targetGridPos);
            MergeableItem targetItem = targetCell.GetItem();

            // Try to merge first
            if (targetItem != null && draggedItem.CanMergeWith(targetItem))
            {
                mergeManager.MergeItems(draggedItem, targetItem, targetGridPos);
                placed = true;
                Debug.Log($"Merged items at {targetGridPos}");
            }
            // Otherwise try to place in empty cell
            else if (targetItem == null)
            {
                gridManager.PlaceItem(draggedItem, targetGridPos);
                draggedItem.transform.position = new Vector3(
                    draggedItem.transform.position.x,
                    draggedItem.transform.position.y,
                    0f
                );

                // Apply place visual effect
                ItemVisualEffects effects = draggedItem.GetComponent<ItemVisualEffects>();
                if (effects != null)
                {
                    effects.OnPlace();
                }

                placed = true;
                Debug.Log($"Placed item at {targetGridPos}");
            }
        }

        // If couldn't place, return to original position
        if (!placed)
        {
            gridManager.PlaceItem(draggedItem, originalGridPosition);
            draggedItem.transform.position = new Vector3(
                draggedItem.transform.position.x,
                draggedItem.transform.position.y,
                0f
            );

            // Apply place visual effect
            ItemVisualEffects effects = draggedItem.GetComponent<ItemVisualEffects>();
            if (effects != null)
            {
                effects.OnPlace();
            }

            Debug.Log($"Returned item to {originalGridPosition}");
        }

        gridManager.ResetAllCellVisuals();
        draggedItem = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
