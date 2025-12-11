using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI coinRewardText;
    public Image backgroundImage;
    public Button button;

    [Header("Visual Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    public Color availableColor = new Color(1f, 1f, 1f, 1f);
    public Color highlightColor = new Color(0.8f, 1f, 0.8f, 1f);

    private OrderData currentOrder;
    private bool isAvailable = false;

    public OrderData CurrentOrder => currentOrder;
    public bool IsAvailable => isAvailable;

    private void Awake()
    {
        Debug.Log($"OrderSlot: Awake called. button is {(button == null ? "NULL" : "assigned")}");
    }

    public void SetupButton()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnOrderClicked);
            Debug.Log($"OrderSlot: Added click listener to button. Button is interactable: {button.interactable}");
        }
        else
        {
            Debug.LogError("OrderSlot: Button is NULL in SetupButton!");
        }
    }

    public void SetOrder(OrderData order)
    {
        currentOrder = order;

        if (order != null)
        {
            // Set item icon
            if (itemIcon != null && order.requiredItem != null)
            {
                itemIcon.sprite = order.requiredItem.sprite;
                itemIcon.enabled = true;
            }

            // Set coin reward text
            if (coinRewardText != null)
            {
                coinRewardText.text = order.coinReward.ToString();
            }

            gameObject.SetActive(true);
            UpdateVisualState(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateVisualState(bool available)
    {
        isAvailable = available;

        if (backgroundImage != null)
        {
            backgroundImage.color = available ? availableColor : normalColor;
        }

        if (button != null)
        {
            button.interactable = available;
        }
    }

    public void Highlight(bool highlight)
    {
        if (backgroundImage != null && isAvailable)
        {
            backgroundImage.color = highlight ? highlightColor : availableColor;
        }
    }

    private void OnOrderClicked()
    {
        Debug.Log($"OrderSlot: OnOrderClicked called! isAvailable={isAvailable}, currentOrder={currentOrder?.name ?? "null"}");

        if (isAvailable && currentOrder != null)
        {
            // Notify the OrderManager
            OrderManager orderManager = FindFirstObjectByType<OrderManager>();
            Debug.Log($"OrderSlot: Found OrderManager: {orderManager != null}");

            if (orderManager != null)
            {
                Debug.Log($"OrderSlot: Calling CompleteOrder for {currentOrder.name}");
                orderManager.CompleteOrder(this);
            }
            else
            {
                Debug.LogError("OrderSlot: OrderManager not found!");
            }
        }
        else
        {
            Debug.LogWarning($"OrderSlot: Cannot complete - isAvailable={isAvailable}, hasOrder={currentOrder != null}");
        }
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
