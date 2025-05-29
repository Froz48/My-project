using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public float health;
    public InventorySlotSaveData[] inventory;
    public InventorySlotSaveData[] equipment;

    public PlayerSaveData(Player player)
    {
        position = player.transform.position;
        health = player.getCurrentHealth();

        inventory = new InventorySlotSaveData[player.GetInventory().Slots.Length];
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InventorySlotSaveData(player.GetInventory().Slots[i]);
        }
        equipment = new InventorySlotSaveData[player.GetEquipment().Slots.Length];
        for (int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = new InventorySlotSaveData(player.GetEquipment().Slots[i]);
        }
    }


}