using UnityEngine;
using TMPro;
using System.Collections;

public class CoinWallet : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinText;
    public RectTransform walletIcon;

    [Header("Animation Settings")]
    public float coinFlyDuration = 0.8f;
    public AnimationCurve coinFlyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int currentCoins = 0;

    private void Start()
    {
        UpdateCoinDisplay();
    }

    public void AddCoins(int amount, Vector3 startWorldPosition)
    {
        currentCoins += amount;
        UpdateCoinDisplay();

        // Animate coin flying from start position to wallet
        StartCoroutine(AnimateCoinFly(startWorldPosition));
    }

    public int GetCoins()
    {
        return currentCoins;
    }

    private void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    private IEnumerator AnimateCoinFly(Vector3 worldStartPosition)
    {
        // Create a temporary coin icon that flies to the wallet
        GameObject coinFlyObj = new GameObject("CoinFly");
        coinFlyObj.transform.SetParent(transform.parent, false);

        RectTransform coinRect = coinFlyObj.AddComponent<RectTransform>();
        UnityEngine.UI.Image coinImage = coinFlyObj.AddComponent<UnityEngine.UI.Image>();

        // Set up the coin visual (you can set a sprite here)
        coinRect.sizeDelta = new Vector2(50, 50);

        // Convert world position to screen position
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 startScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Camera.main.WorldToScreenPoint(worldStartPosition),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out startScreenPos
        );

        coinRect.anchoredPosition = startScreenPos;

        // Get wallet position
        Vector2 endPos = walletIcon.anchoredPosition;

        // Animate
        float elapsed = 0f;
        while (elapsed < coinFlyDuration)
        {
            elapsed += Time.deltaTime;
            float t = coinFlyCurve.Evaluate(elapsed / coinFlyDuration);

            coinRect.anchoredPosition = Vector2.Lerp(startScreenPos, endPos, t);
            coinRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, t);

            yield return null;
        }

        // Destroy the flying coin
        Destroy(coinFlyObj);

        // Optional: Scale pulse effect on wallet
        if (walletIcon != null)
        {
            StartCoroutine(PulseWallet());
        }
    }

    private IEnumerator PulseWallet()
    {
        Vector3 originalScale = walletIcon.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        float duration = 0.2f;
        float elapsed = 0f;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            walletIcon.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        // Scale down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            walletIcon.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            yield return null;
        }

        walletIcon.localScale = originalScale;
    }
}
