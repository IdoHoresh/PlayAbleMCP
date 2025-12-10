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
                // Save current position before PlaceItem moves it
                Vector3 currentDraggedPosition = draggedItem.transform.position;
                Debug.Log($"[DRAG DEBUG] Saved dragged position: {currentDraggedPosition}");

                gridManager.PlaceItem(draggedItem, targetGridPos);

                // Get target world position
                Vector2 targetWorldPos = gridManager.GridToWorldPosition(targetGridPos.x, targetGridPos.y);
                Vector3 finalPosition = new Vector3(targetWorldPos.x, targetWorldPos.y, 0f);
                Debug.Log($"[DRAG DEBUG] Target grid position: {finalPosition}");

                // Apply place visual effect with snap animation
                ItemVisualEffects effects = draggedItem.GetComponent<ItemVisualEffects>();
                if (effects != null)
                {
                    // Move back to dragged position, then animate to final position
                    draggedItem.transform.position = currentDraggedPosition;
                    Debug.Log($"[DRAG DEBUG] Moved item back to: {draggedItem.transform.position}");
                    effects.OnPlace();
                    effects.PlaySnapToGridAnimation(finalPosition);
                }
                else
                {
                    // Fallback if no effects component
                    draggedItem.transform.position = finalPosition;
                }

                placed = true;
                Debug.Log($"Placed item at {targetGridPos}");
            }
        }

        // If couldn't place, return to original position
        if (!placed)
        {
            // Save current position before PlaceItem moves it
            Vector3 currentDraggedPosition = draggedItem.transform.position;

            gridManager.PlaceItem(draggedItem, originalGridPosition);

            // Get original world position
            Vector2 originalWorldPos = gridManager.GridToWorldPosition(originalGridPosition.x, originalGridPosition.y);
            Vector3 finalPosition = new Vector3(originalWorldPos.x, originalWorldPos.y, 0f);

            // Apply place visual effect with snap animation
            ItemVisualEffects effects = draggedItem.GetComponent<ItemVisualEffects>();
            if (effects != null)
            {
                // Move back to dragged position, then animate to final position
                draggedItem.transform.position = currentDraggedPosition;
                effects.OnPlace();
                effects.PlaySnapToGridAnimation(finalPosition);
            }
            else
            {
                // Fallback if no effects component
                draggedItem.transform.position = finalPosition;
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
