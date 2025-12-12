using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Animates item delivery from order slot to character
/// Similar to coin animation but delivers items to the character
/// </summary>
public class ItemDeliveryAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float deliveryDuration = 2f;
    public AnimationCurve deliveryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Item Visual Settings")]
    public Vector2 itemIconSize = new Vector2(120, 120);
    public float rotationSpeed = 180f; // Degrees per second
    
    [Header("References")]
    public CharacterViewController characterViewController;
    public Canvas canvas;

    private void Awake()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (characterViewController == null)
        {
            characterViewController = FindFirstObjectByType<CharacterViewController>();
        }
    }

    /// <summary>
    /// Animate an item from order slot to character
    /// </summary>
    public void DeliverItem(OrderSlot orderSlot, OrderData orderData, System.Action onComplete = null)
    {
        if (orderSlot == null || orderData == null)
        {
            Debug.LogError("ItemDeliveryAnimator: Invalid order slot or order data");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(AnimateItemDelivery(orderSlot, orderData, onComplete));
    }

    private IEnumerator AnimateItemDelivery(OrderSlot orderSlot, OrderData orderData, System.Action onComplete)
    {
        // Create flying item icon
        GameObject flyingItem = CreateFlyingItemIcon(orderData);
        
        if (flyingItem == null)
        {
            Debug.LogError("ItemDeliveryAnimator: Failed to create flying item");
            onComplete?.Invoke();
            yield break;
        }

        RectTransform itemRect = flyingItem.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Get start position (order slot position in screen space)
        Vector2 startPos = GetScreenPosition(orderSlot.transform, canvasRect);

        // Get end position (character position in screen space)
        Vector2 endPos = GetCharacterScreenPosition(canvasRect);

        itemRect.anchoredPosition = startPos;

        // Animate to character
        float elapsed = 0f;
        Quaternion startRotation = itemRect.rotation;

        while (elapsed < deliveryDuration)
        {
            elapsed += Time.deltaTime;
            float t = deliveryCurve.Evaluate(elapsed / deliveryDuration);

            // Move towards character
            itemRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Scale down as it gets closer
            float scale = Mathf.Lerp(1f, 0.5f, t);
            itemRect.localScale = Vector3.one * scale;

            // Rotate for visual interest
            float rotation = rotationSpeed * elapsed;
            itemRect.rotation = startRotation * Quaternion.Euler(0, 0, rotation);

            yield return null;
        }

        // Destroy flying item
        Destroy(flyingItem);

        // Trigger character receiving animation
        if (characterViewController != null)
        {
            CharacterAnimationController animController = characterViewController.GetAnimationController();
            CharacterEquipmentManager equipManager = characterViewController.GetEquipmentManager();

            Debug.Log($"ItemDeliveryAnimator: animController={animController != null}, equipManager={equipManager != null}, itemPrefab3D={orderData.itemPrefab3D != null}, equipmentSlot={orderData.equipmentSlot}");

            if (animController != null)
            {
                // Play receive animation
                animController.PlayReceiveItemAnimation(orderData.itemType, () =>
                {
                    Debug.Log($"ItemDeliveryAnimator: Animation complete callback - equipManager={equipManager != null}, itemPrefab3D={orderData.itemPrefab3D != null}");
                    // After receive animation, equip the item
                    if (equipManager != null && orderData.itemPrefab3D != null)
                    {
                        equipManager.EquipItem(
                            orderData.itemType,
                            orderData.itemPrefab3D,
                            orderData.equipmentSlot,
                            orderData.equipmentPositionOffset,
                            orderData.equipmentRotationOffset,
                            orderData.equipmentScale
                        );
                    }
                    else
                    {
                        Debug.LogWarning($"ItemDeliveryAnimator: Cannot equip - equipManager={equipManager != null}, itemPrefab3D={orderData.itemPrefab3D != null}");
                    }
                });
            }
            else if (equipManager != null && orderData.itemPrefab3D != null)
            {
                Debug.Log("ItemDeliveryAnimator: No animation controller, equipping immediately");
                // No animation controller, just equip immediately
                equipManager.EquipItem(
                    orderData.itemType,
                    orderData.itemPrefab3D,
                    orderData.equipmentSlot,
                    orderData.equipmentPositionOffset,
                    orderData.equipmentRotationOffset,
                    orderData.equipmentScale
                );
            }
            else
            {
                Debug.LogWarning($"ItemDeliveryAnimator: Cannot equip (no anim path) - equipManager={equipManager != null}, itemPrefab3D={orderData.itemPrefab3D != null}");
            }
        }
        else
        {
            Debug.LogError("ItemDeliveryAnimator: characterViewController is NULL!");
        }

        // Callback
        onComplete?.Invoke();
    }

    private GameObject CreateFlyingItemIcon(OrderData orderData)
    {
        if (canvas == null)
        {
            Debug.LogError("ItemDeliveryAnimator: No canvas found!");
            return null;
        }

        GameObject itemObj = new GameObject("FlyingItem");
        itemObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = itemObj.AddComponent<RectTransform>();
        rect.sizeDelta = itemIconSize;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image image = itemObj.AddComponent<Image>();
        
        // Use the item icon from order data
        if (orderData.requiredItem != null && orderData.requiredItem.sprite != null)
        {
            image.sprite = orderData.requiredItem.sprite;
        }
        else
        {
            // Fallback: white square
            image.color = Color.white;
        }

        // Add glow/outline effect
        var outline = itemObj.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.8f, 0f, 1f); // Gold outline
        outline.effectDistance = new Vector2(3, 3);

        return itemObj;
    }

    private Vector2 GetScreenPosition(Transform worldTransform, RectTransform canvasRect)
    {
        Vector2 screenPos;
        
        // Convert world position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Camera.main.WorldToScreenPoint(worldTransform.position),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out screenPos
        );

        return screenPos;
    }

    private Vector2 GetCharacterScreenPosition(RectTransform canvasRect)
    {
        if (characterViewController != null)
        {
            Vector3 characterWorldPos = characterViewController.GetCharacterWorldPosition();
            
            Vector2 screenPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Camera.main.WorldToScreenPoint(characterWorldPos + Vector3.up * 1.2f), // Aim at character's chest
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                out screenPos
            );

            return screenPos;
        }

        // Fallback: right side of screen
        return new Vector2(canvasRect.rect.width * 0.25f, 0);
    }

    /// <summary>
    /// Quick delivery without animation (for testing or instant delivery)
    /// </summary>
    public void DeliverItemInstant(OrderData orderData)
    {
        if (characterViewController == null)
        {
            Debug.LogError("ItemDeliveryAnimator: No CharacterViewController found!");
            return;
        }

        CharacterEquipmentManager equipManager = characterViewController.GetEquipmentManager();
        if (equipManager != null && orderData.itemPrefab3D != null)
        {
            equipManager.EquipItem(
                orderData.itemType,
                orderData.itemPrefab3D,
                orderData.equipmentSlot,
                orderData.equipmentPositionOffset,
                orderData.equipmentRotationOffset,
                orderData.equipmentScale
            );
            Debug.Log($"ItemDeliveryAnimator: Instantly equipped {orderData.itemType}");
        }
    }
}
