using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class WorldSaveData
{
    public PlayerData[] players;

}

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public float health;
    public SlotData[] inventory;
    public SlotData[] equipment;

    public PlayerData(Player player)
    {
        position = player.transform.position;
        health = player.getCurrentHealth();

        inventory = new SlotData[player.GetInventory().Slots.Length];
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new SlotData(player.GetInventory().Slots[i]);
        }
        equipment = new SlotData[player.GetEquipment().Slots.Length];
        for (int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = new SlotData(player.GetEquipment().Slots[i]);
        }
    }


}

[System.Serializable]
public class SlotData
{
    public int id;
    public int amount;

    public SlotData(InventorySlot slot)
    {
        if (slot.item)
        {
            id = slot.item.id;
            amount = slot.amount;
        }

    }
}


public class SaveManager : MonoBehaviour
{
    public static string CurrentWorldName => PlayerPrefs.GetString("CurrentWorld", "default_world");
    public static Database itemDb;
    public void Start()
    {
        if (itemDb == null)
        {
            itemDb = Resources.Load("ItemDatabase") as Database;
        }
    }
    public static void SaveCurrentWorld()
    {
        SaveWorld(CurrentWorldName);
    }
    public static void LoadCurrentWorld()
    {
        string worldName = CurrentWorldName;
        if (!File.Exists(Path.Combine(Application.persistentDataPath, worldName + ".json")))
        {
            // Создаем новый мир, если он не существует
            WorldSaveData initialData = new WorldSaveData
            {
                players = new PlayerData[0]
            };
            string json = JsonUtility.ToJson(initialData);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, worldName + ".json"), json);
        }
        
        LoadGame(worldName);
    }
    public static void SaveWorld(string saveName)
    {
        WorldSaveData saveData = new WorldSaveData();

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        saveData.players = new PlayerData[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            saveData.players[i] = new PlayerData(players[i]);
        }



        string json = JsonUtility.ToJson(saveData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + saveName + ".json", json);
        Debug.Log(Application.persistentDataPath + "/" + saveName + ".json");
    }

    public static void LoadGame(string saveName)
    {
        string path = Application.persistentDataPath + "/" + saveName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            // Восстановление игроков
            foreach (PlayerData playerData in data.players)
            {
                Player player = FindAnyObjectByType<Player>();
                LoadToPlayer(player, playerData);
            }
        }
    }
    public static void LoadToPlayer(Player player, PlayerData data)
    {
        player.transform.position = data.position;
        player.SetCurrentHealth(data.health);

        
        for (int i = 0; i < data.inventory.Length; i++)
        {
            if (data.inventory[i].amount > 0) 
            {
                Item item = itemDb.GetObjectById(data.inventory[i].id) as Item;
                player.GetInventory().Slots[i].UpdateSlot(item, data.inventory[i].amount);
            }
        }
        for (int i = 0; i < data.equipment.Length; i++)
        {
            if (data.equipment[i].amount > 0) 
            {
                Item item = itemDb.GetObjectById(data.equipment[i].id) as Item;
                player.GetEquipment().Slots[i].UpdateSlot(item, data.equipment[i].amount);
            }
        }
    }
}

