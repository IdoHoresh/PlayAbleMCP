using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages equipment on the character
/// Handles showing/hiding equipment meshes and attachment to bones
/// </summary>
public class CharacterEquipmentManager : MonoBehaviour
{
    [Header("Equipment Slots")]
    public Transform rightHandSlot;
    public Transform leftHandSlot;
    public Transform backSlot;
    public Transform headSlot;
    public Transform bodySlot;

    [Header("Current Equipment")]
    private Dictionary<EquipmentSlot, GameObject> equippedItems = new Dictionary<EquipmentSlot, GameObject>();

    [Header("Debug")]
    public bool showDebugInfo = true;

    private Animator characterAnimator;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
        InitializeEquipmentSlots();
    }

    /// <summary>
    /// Find or create attachment points on the character
    /// </summary>
    private void InitializeEquipmentSlots()
    {
        if (characterAnimator == null)
        {
            Debug.LogWarning("CharacterEquipmentManager: No Animator found. Equipment slots will need to be manually assigned.");
            return;
        }

        // Try to find standard bone attachment points from Mixamo rig
        // These bone names are standard for Mixamo humanoid characters
        
        if (rightHandSlot == null)
            rightHandSlot = FindOrCreateAttachmentPoint("RightHand", "AttachPoint_RightHand");
        
        if (leftHandSlot == null)
            leftHandSlot = FindOrCreateAttachmentPoint("LeftHand", "AttachPoint_LeftHand");
        
        if (backSlot == null)
            backSlot = FindOrCreateAttachmentPoint("Spine2", "AttachPoint_Back");
        
        if (headSlot == null)
            headSlot = FindOrCreateAttachmentPoint("Head", "AttachPoint_Head");

        if (showDebugInfo)
        {
            Debug.Log($"CharacterEquipmentManager: Initialized equipment slots - " +
                     $"RightHand: {rightHandSlot != null}, " +
                     $"LeftHand: {leftHandSlot != null}, " +
                     $"Back: {backSlot != null}, " +
                     $"Head: {headSlot != null}");
        }
    }

    /// <summary>
    /// Find a bone by name and create an attachment point child
    /// </summary>
    private Transform FindOrCreateAttachmentPoint(string boneName, string attachPointName)
    {
        // Search for bone in character hierarchy
        Transform bone = FindDeepChild(transform, boneName);
        
        if (bone == null)
        {
            Debug.LogWarning($"CharacterEquipmentManager: Could not find bone '{boneName}'");
            return null;
        }

        // Check if attachment point already exists
        Transform attachPoint = bone.Find(attachPointName);
        if (attachPoint == null)
        {
            // Create new attachment point
            GameObject attachObj = new GameObject(attachPointName);
            attachObj.transform.SetParent(bone, false);
            attachObj.transform.localPosition = Vector3.zero;
            attachObj.transform.localRotation = Quaternion.identity;
            attachPoint = attachObj.transform;
        }

        return attachPoint;
    }

    /// <summary>
    /// Recursively search for a child transform by name
    /// </summary>
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name)) // Contains check for variations like "mixamorig:RightHand"
                return child;
            
            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// Equip an item on the character
    /// </summary>
    public void EquipItem(ItemType itemType, GameObject itemPrefab, EquipmentSlot slot, Vector3 positionOffset = default, Vector3 rotationOffset = default, Vector3 scale = default)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning($"CharacterEquipmentManager: Cannot equip {itemType} - no prefab provided");
            return;
        }

        Transform attachPoint = GetAttachmentPoint(slot);
        if (attachPoint == null)
        {
            Debug.LogWarning($"CharacterEquipmentManager: No attachment point for slot {slot}");
            return;
        }

        // Unequip existing item in this slot
        UnequipItem(slot);

        // Instantiate new item
        GameObject equippedItem = Instantiate(itemPrefab, attachPoint);

        // Apply position offset
        equippedItem.transform.localPosition = positionOffset;

        // Apply rotation offset
        equippedItem.transform.localRotation = Quaternion.Euler(rotationOffset);

        // Apply scale (use Vector3.one if not specified)
        equippedItem.transform.localScale = scale == Vector3.zero ? Vector3.one : scale;

        equippedItem.name = $"Equipped_{itemType}";

        // Store in dictionary
        equippedItems[slot] = equippedItem;

        // Set to Character layer
        SetLayerRecursively(equippedItem, gameObject.layer);

        if (showDebugInfo)
        {
            Debug.Log($"CharacterEquipmentManager: Equipped {itemType} in {slot} slot at position={positionOffset}, rotation={rotationOffset}, scale={scale}");
        }
    }

    /// <summary>
    /// Unequip an item from a slot
    /// </summary>
    public void UnequipItem(EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot) && equippedItems[slot] != null)
        {
            Destroy(equippedItems[slot]);
            equippedItems.Remove(slot);
            
            if (showDebugInfo)
            {
                Debug.Log($"CharacterEquipmentManager: Unequipped item from {slot} slot");
            }
        }
    }

    /// <summary>
    /// Get the currently equipped item in a slot
    /// </summary>
    public GameObject GetEquippedItem(EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot))
        {
            return equippedItems[slot];
        }
        return null;
    }

    /// <summary>
    /// Check if a slot has an item equipped
    /// </summary>
    public bool HasItemEquipped(EquipmentSlot slot)
    {
        return equippedItems.ContainsKey(slot) && equippedItems[slot] != null;
    }

    /// <summary>
    /// Get the attachment point transform for a given slot
    /// </summary>
    private Transform GetAttachmentPoint(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.RightHand:
                return rightHandSlot;
            case EquipmentSlot.LeftHand:
                return leftHandSlot;
            case EquipmentSlot.Back:
                return backSlot;
            case EquipmentSlot.Head:
                return headSlot;
            case EquipmentSlot.Body:
                return bodySlot;
            default:
                return null;
        }
    }

    /// <summary>
    /// Show or hide an equipped item
    /// </summary>
    public void SetEquipmentVisible(EquipmentSlot slot, bool visible)
    {
        if (equippedItems.ContainsKey(slot) && equippedItems[slot] != null)
        {
            equippedItems[slot].SetActive(visible);
        }
    }

    /// <summary>
    /// Clear all equipped items
    /// </summary>
    public void UnequipAll()
    {
        foreach (var kvp in equippedItems)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        equippedItems.Clear();
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Visualize attachment points in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        DrawAttachmentGizmo(rightHandSlot, Color.red);
        DrawAttachmentGizmo(leftHandSlot, Color.blue);
        DrawAttachmentGizmo(backSlot, Color.green);
        DrawAttachmentGizmo(headSlot, Color.yellow);
        DrawAttachmentGizmo(bodySlot, Color.cyan);
    }

    private void DrawAttachmentGizmo(Transform attachPoint, Color color)
    {
        if (attachPoint == null) return;

        Gizmos.color = color;
        Gizmos.DrawWireSphere(attachPoint.position, 0.05f);
        Gizmos.DrawLine(attachPoint.position, attachPoint.position + attachPoint.forward * 0.1f);
    }
#endif
}
