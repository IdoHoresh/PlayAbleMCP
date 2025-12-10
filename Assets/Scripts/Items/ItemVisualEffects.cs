using UnityEngine;

public class ItemVisualEffects : MonoBehaviour
{
    private Vector3 originalScale;
    private Transform itemTransform;
    
    [Header("Scale Settings")]
    public float pickupScale = 1.2f;
    public float scaleSpeed = 8f;

    [Header("Snap Animation Settings")]
    public float snapDuration = 0.2f;
    
    private bool isPickedUp = false;
    private Vector3 targetScale;

    private void Awake()
    {
        itemTransform = transform;
        originalScale = itemTransform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smoothly interpolate to target scale
        if (itemTransform.localScale != targetScale)
        {
            itemTransform.localScale = Vector3.Lerp(
                itemTransform.localScale, 
                targetScale, 
                Time.deltaTime * scaleSpeed
            );
        }
    }

    public void OnPickup()
    {
        isPickedUp = true;
        targetScale = originalScale * pickupScale;
    }

    public void OnPlace()
    {
        isPickedUp = false;
        targetScale = originalScale;
    }

    public void PlayMergeEffect()
    {
        // Scale up then destroy
        StartCoroutine(MergeScaleEffect());
    }

    private System.Collections.IEnumerator MergeScaleEffect()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 startScale = itemTransform.localScale;
        Vector3 endScale = originalScale * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            itemTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }

    public void PlaySpawnEffect()
    {
        // Pop in effect
        StartCoroutine(SpawnScaleEffect());
    }

    public void PlaySnapToGridAnimation(Vector3 targetPosition)
    {
        StartCoroutine(SnapToGridCoroutine(targetPosition));
    }

    private System.Collections.IEnumerator SnapToGridCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = itemTransform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);

        Debug.Log($"[SNAP ANIMATION] Starting snap animation. Distance: {distance:F3}, Duration: {snapDuration}s");
        Debug.Log($"[SNAP ANIMATION] Start: {startPosition}, Target: {targetPosition}");

        float elapsed = 0f;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / snapDuration;
            // Ease out quad: starts fast, slows at end
            float eased = 1f - (1f - t) * (1f - t);

            itemTransform.position = Vector3.Lerp(startPosition, targetPosition, eased);
            yield return null;
        }

        itemTransform.position = targetPosition; // Ensure exact final position
        Debug.Log($"[SNAP ANIMATION] Snap animation complete");
    }

    private System.Collections.IEnumerator SpawnScaleEffect()
    {
        itemTransform.localScale = Vector3.zero;
        targetScale = originalScale;

        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out elastic
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            itemTransform.localScale = originalScale * scale;
            yield return null;
        }

        itemTransform.localScale = originalScale;
    }
}
