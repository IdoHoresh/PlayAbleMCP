using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetup : MonoBehaviour
{
    [Header("References")]
    public OrderManager orderManager;
    public GridManager gridManager;

    [Header("Order Data")]
    public OrderData[] orders;

    private Canvas canvas;
    private CoinWallet coinWallet;
    private OrderSlot[] orderSlots;

    private void Awake()
    {
        Debug.Log("UISetup: Awake called, starting UI setup...");
        SetupUI();
    }

    private void SetupUI()
    {
        Debug.Log($"UISetup: SetupUI starting. orderManager = {(orderManager == null ? "null" : "assigned")}, gridManager = {(gridManager == null ? "null" : "assigned")}, orders count = {(orders == null ? "null" : orders.Length.ToString())}");

        // Create Canvas
        canvas = CreateCanvas();
        Debug.Log("UISetup: Canvas created");

        // Create Orders Panel at top of screen
        GameObject ordersPanel = CreateOrdersPanel(canvas.transform);

        // Create Coin Wallet on top-right
        GameObject walletObj = CreateCoinWallet(canvas.transform);
        coinWallet = walletObj.GetComponent<CoinWallet>();
        Debug.Log($"UISetup: Orders panel and wallet created. orderSlots count = {orderSlots.Length}");

        // Setup OrderManager references
        if (orderManager != null)
        {
            orderManager.gridManager = gridManager;
            orderManager.coinWallet = coinWallet;
            orderManager.orderSlots = orderSlots;
            orderManager.availableOrders = orders;
            Debug.Log($"UISetup: OrderManager references set - gridManager: {(orderManager.gridManager != null ? "set" : "null")}, coinWallet: {(orderManager.coinWallet != null ? "set" : "null")}, orderSlots: {orderManager.orderSlots.Length}, orders: {orderManager.availableOrders.Length}");
        }
        else
        {
            Debug.LogError("UISetup: orderManager is NULL! Cannot assign references.");
        }

        Debug.Log("UISetup: Setup complete");
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canv = canvasObj.AddComponent<Canvas>();
        canv.renderMode = RenderMode.ScreenSpaceOverlay;
        canv.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create EventSystem if it doesn't exist
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        return canv;
    }

    private Vector2 CalculateGridTopCenter()
    {
        if (gridManager == null)
        {
            Debug.LogWarning("UISetup: GridManager is null, using default position");
            return new Vector2(0, 2f); // Fallback position
        }

        // Calculate the top center of the grid in world space
        float gridWidth = gridManager.gridWidth * gridManager.cellSize;
        float gridHeight = gridManager.gridHeight * gridManager.cellSize;

        // Grid is centered around gridOffset, so top is gridOffset.y + (gridHeight / 2)
        float topY = gridManager.gridOffset.y + (gridHeight / 2f);
        float centerX = gridManager.gridOffset.x;

        return new Vector2(centerX, topY);
    }

    private GameObject CreateOrdersPanel(Transform parent)
    {
        // Create panel
        GameObject panel = new GameObject("OrdersPanel");
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();

        // Anchor to top-center of screen
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f); // Pivot at top center of panel

        // Position at top of screen with small margin
        panelRect.anchoredPosition = new Vector2(0, -20); // 20 pixels down from top
        panelRect.sizeDelta = new Vector2(950, 160);

        Debug.Log($"UISetup: Orders panel positioned at top of screen: {panelRect.anchoredPosition}");

        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(1f, 1f, 1f, 0.05f);

        // Create 3 order slots with better spacing
        orderSlots = new OrderSlot[3];
        float slotWidth = 280f;
        float spacing = 25f;
        float totalWidth = slotWidth * 3 + spacing * 2;
        float startX = -totalWidth / 2f + slotWidth / 2f;

        for (int i = 0; i < 3; i++)
        {
            float xPos = startX + i * (slotWidth + spacing);
            orderSlots[i] = CreateOrderSlot(panel.transform, xPos, slotWidth);
        }

        return panel;
    }

    private OrderSlot CreateOrderSlot(Transform parent, float xPos, float width)
    {
        GameObject slotObj = new GameObject("OrderSlot");
        slotObj.transform.SetParent(parent, false);

        RectTransform slotRect = slotObj.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0.5f, 0.5f);
        slotRect.anchorMax = new Vector2(0.5f, 0.5f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.anchoredPosition = new Vector2(xPos, 0);
        slotRect.sizeDelta = new Vector2(width, 150);

        Image slotBg = slotObj.AddComponent<Image>();
        slotBg.color = new Color(1f, 0.9f, 0.8f, 0.5f);
        slotBg.sprite = CreateRoundedSprite();
        slotBg.raycastTarget = true; // Ensure the image can receive raycasts

        Button button = slotObj.AddComponent<Button>();
        button.targetGraphic = slotBg; // Set the background image as the button's target graphic
        button.interactable = true; // Explicitly set button as interactable

        // Create item icon
        GameObject iconObj = new GameObject("ItemIcon");
        iconObj.transform.SetParent(slotObj.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0, 15);
        iconRect.sizeDelta = new Vector2(80, 80);
        Image itemIcon = iconObj.AddComponent<Image>();

        // Create coin reward text
        GameObject coinTextObj = new GameObject("CoinText");
        coinTextObj.transform.SetParent(slotObj.transform, false);
        RectTransform coinTextRect = coinTextObj.AddComponent<RectTransform>();
        coinTextRect.anchorMin = new Vector2(0.5f, 0f);
        coinTextRect.anchorMax = new Vector2(0.5f, 0f);
        coinTextRect.pivot = new Vector2(0.5f, 0f);
        coinTextRect.anchoredPosition = new Vector2(0, 10);
        coinTextRect.sizeDelta = new Vector2(100, 40);

        TextMeshProUGUI coinText = coinTextObj.AddComponent<TextMeshProUGUI>();
        coinText.text = "50";
        coinText.fontSize = 32;
        coinText.fontStyle = FontStyles.Bold;
        coinText.color = Color.yellow;
        coinText.alignment = TextAlignmentOptions.Center;

        // Add OrderSlot component
        OrderSlot orderSlot = slotObj.AddComponent<OrderSlot>();
        orderSlot.itemIcon = itemIcon;
        orderSlot.coinRewardText = coinText;
        orderSlot.backgroundImage = slotBg;
        orderSlot.button = button;

        // Manually add the click listener since Awake already ran before we assigned the button
        orderSlot.SetupButton();

        Debug.Log($"UISetup: Created OrderSlot. Button assigned: {button != null}, Button interactable: {button.interactable}");

        return orderSlot;
    }

    private GameObject CreateCoinWallet(Transform parent)
    {
        GameObject walletObj = new GameObject("CoinWallet");
        walletObj.transform.SetParent(parent, false);

        RectTransform walletRect = walletObj.AddComponent<RectTransform>();
        // Position wallet in top-right corner instead of center-right
        walletRect.anchorMin = new Vector2(1f, 1f);
        walletRect.anchorMax = new Vector2(1f, 1f);
        walletRect.pivot = new Vector2(1f, 1f);
        walletRect.anchoredPosition = new Vector2(-20, -20); // 20px from top-right corner
        walletRect.sizeDelta = new Vector2(150, 150);

        Image walletBg = walletObj.AddComponent<Image>();
        walletBg.color = new Color(1f, 0.85f, 0.3f, 0.8f);
        walletBg.sprite = CreateRoundedSprite();

        // Create wallet icon placeholder
        GameObject iconObj = new GameObject("WalletIcon");
        iconObj.transform.SetParent(walletObj.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0, 20);
        iconRect.sizeDelta = new Vector2(60, 60);

        Image walletIcon = iconObj.AddComponent<Image>();
        walletIcon.color = new Color(0.8f, 0.6f, 0.2f);

        // Create coin count text
        GameObject textObj = new GameObject("CoinText");
        textObj.transform.SetParent(walletObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0f);
        textRect.anchorMax = new Vector2(0.5f, 0f);
        textRect.pivot = new Vector2(0.5f, 0f);
        textRect.anchoredPosition = new Vector2(0, 20);
        textRect.sizeDelta = new Vector2(120, 40);

        TextMeshProUGUI coinText = textObj.AddComponent<TextMeshProUGUI>();
        coinText.text = "0";
        coinText.fontSize = 36;
        coinText.fontStyle = FontStyles.Bold;
        coinText.color = Color.white;
        coinText.alignment = TextAlignmentOptions.Center;

        // Add CoinWallet component
        CoinWallet wallet = walletObj.AddComponent<CoinWallet>();
        wallet.coinText = coinText;
        wallet.walletIcon = iconRect;

        return walletObj;
    }

    private Sprite CreateRoundedSprite()
    {
        // Create a simple white sprite (Unity will handle rounding with the UI system)
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
        return sprite;
    }
}
