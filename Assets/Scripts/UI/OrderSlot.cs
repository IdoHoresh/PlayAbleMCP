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
        if (button != null)
        {
            button.onClick.AddListener(OnOrderClicked);
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
        if (isAvailable && currentOrder != null)
        {
            // Notify the OrderManager
            OrderManager orderManager = FindFirstObjectByType<OrderManager>();
            if (orderManager != null)
            {
                orderManager.CompleteOrder(this);
            }
        }
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
