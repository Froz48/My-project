[System.Serializable]
public class InventorySlotSaveData
{
    public int id;
    public int amount;

    public InventorySlotSaveData(InventorySlot slot)
    {
        if (slot.item)
        {
            id = slot.item.id;
            amount = slot.amount;
        }

    }
}
