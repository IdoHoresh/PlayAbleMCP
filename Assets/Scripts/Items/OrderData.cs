using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder", menuName = "Merge Game/Order Data")]
public class OrderData : ScriptableObject
{
    [Header("Order Requirements")]
    public ItemData requiredItem;
    public int quantity = 1;

    [Header("Rewards")]
    public int coinReward = 10;

    [Header("Visual")]
    public Sprite itemIcon;

    public string GetDescription()
    {
        return $"{quantity}x {requiredItem.itemName}";
    }
}
