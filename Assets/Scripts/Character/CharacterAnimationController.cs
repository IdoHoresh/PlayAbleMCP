using UnityEngine;
using System.Collections;

/// <summary>
/// Controls character animations based on game events
/// Handles idle, receive item, equip item, and celebration animations
/// </summary>
[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [Header("Animation References")]
    public Animator animator;
    
    [Header("Animation State Names")]
    public string idleStateName = "Idle";
    public string receiveItemStateName = "ReceiveItem";
    public string celebrateStateName = "Victory";
    
    [Header("Animation Parameters")]
    public string receiveItemTrigger = "ReceiveItem";
    public string celebrateTrigger = "Celebrate";
    
    [Header("Timing")]
    public float receiveItemDuration = 1.5f;
    public float celebrateDuration = 2f;

    private bool isAnimating = false;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("CharacterAnimationController: No Animator component found!");
        }
    }

    private void Start()
    {
        // Start with idle animation
        PlayIdleAnimation();
    }

    /// <summary>
    /// Play the idle animation (default state)
    /// </summary>
    public void PlayIdleAnimation()
    {
        if (animator == null) return;
        
        // Reset to idle - this will happen automatically after other animations complete
        isAnimating = false;
        Debug.Log("CharacterAnimationController: Playing Idle animation");
    }

    /// <summary>
    /// Play the receive item animation
    /// </summary>
    public void PlayReceiveItemAnimation(ItemType itemType, System.Action onComplete = null)
    {
        if (animator == null || isAnimating) return;

        StartCoroutine(PlayAnimationCoroutine(receiveItemTrigger, receiveItemDuration, onComplete));
        Debug.Log($"CharacterAnimationController: Playing ReceiveItem animation for {itemType}");
    }

    /// <summary>
    /// Play the celebration/victory animation
    /// </summary>
    public void PlayCelebrateAnimation(System.Action onComplete = null)
    {
        if (animator == null || isAnimating) return;

        StartCoroutine(PlayAnimationCoroutine(celebrateTrigger, celebrateDuration, onComplete));
        Debug.Log("CharacterAnimationController: Playing Celebrate animation");
    }

    /// <summary>
    /// Generic animation playback with callback
    /// </summary>
    private IEnumerator PlayAnimationCoroutine(string triggerName, float duration, System.Action onComplete)
    {
        isAnimating = true;

        // Trigger the animation
        if (animator.parameterCount > 0)
        {
            // Check if trigger exists
            bool triggerExists = false;
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == triggerName && param.type == AnimatorControllerParameterType.Trigger)
                {
                    triggerExists = true;
                    break;
                }
            }

            if (triggerExists)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogWarning($"CharacterAnimationController: Trigger '{triggerName}' not found in Animator");
            }
        }

        // Wait for animation to complete
        yield return new WaitForSeconds(duration);

        isAnimating = false;

        // Return to idle
        PlayIdleAnimation();

        // Callback
        onComplete?.Invoke();
    }

    /// <summary>
    /// Check if character is currently playing an animation
    /// </summary>
    public bool IsAnimating()
    {
        return isAnimating;
    }

    /// <summary>
    /// Force stop current animation and return to idle
    /// </summary>
    public void StopCurrentAnimation()
    {
        StopAllCoroutines();
        isAnimating = false;
        PlayIdleAnimation();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Debug method to test animations in editor
    /// </summary>
    [ContextMenu("Test Receive Item Animation")]
    private void TestReceiveItem()
    {
        PlayReceiveItemAnimation(ItemType.Axe);
    }

    [ContextMenu("Test Celebrate Animation")]
    private void TestCelebrate()
    {
        PlayCelebrateAnimation();
    }
#endif
}
