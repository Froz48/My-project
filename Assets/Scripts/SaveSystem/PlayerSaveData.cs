using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public string characterGuid;
    public string characterName;
    public ulong ownerClientId;
    public Vector3 position;
    public float health;
    public InventorySlotSaveData[] inventory;
    public InventorySlotSaveData[] equipment;

    public PlayerSaveData(Player player, string name)
    {
        characterGuid = System.Guid.NewGuid().ToString();
        characterName = name;
        ownerClientId = player.OwnerClientId;
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
    public PlayerSaveData(Player player)
    {
        characterGuid = player.GetCharacterGuid();
        characterName = player.GetCharacterName(); 
        ownerClientId = player.OwnerClientId;
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
    public PlayerSaveData()
    {
        characterGuid = System.Guid.NewGuid().ToString();
        characterName = "New Adventurer";
        position = new Vector3(0, 0, -1);
        health = 100f;
        inventory = new InventorySlotSaveData[40];
        for (int i = 0; i < inventory.Length; i++) inventory[i] = new InventorySlotSaveData(new InventorySlot());

        equipment = new InventorySlotSaveData[9];
        for (int i = 0; i < equipment.Length; i++) equipment[i] = new InventorySlotSaveData(new InventorySlot());
    }

}