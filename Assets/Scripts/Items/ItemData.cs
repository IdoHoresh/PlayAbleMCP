using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Merge Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Identity")]
    public string itemName;
    public string itemType; // "wood", "stone", etc.
    public int level = 1;

    [Header("Visual")]
    public Sprite sprite;
    public Color color = Color.white;

    [Header("Merge Settings")]
    public ItemData mergesInto; // The next level item this becomes when merged
    
    public bool CanMergeWith(ItemData other)
    {
        if (other == null) return false;
        
        // Can only merge with same type and same level
        return itemType == other.itemType && level == other.level;
    }
}