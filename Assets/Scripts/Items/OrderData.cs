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

    [Header("Character Equipment")]
    public ItemType itemType = ItemType.None;
    public GameObject itemPrefab3D; // 3D model to equip on character
    public EquipmentSlot equipmentSlot = EquipmentSlot.None;

    [Header("Equipment Transform Offsets")]
    public Vector3 equipmentPositionOffset = Vector3.zero;
    public Vector3 equipmentRotationOffset = Vector3.zero;
    public Vector3 equipmentScale = Vector3.one;

    public string GetDescription()
    {
        return $"{quantity}x {requiredItem.itemName}";
    }
}
