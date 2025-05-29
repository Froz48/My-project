using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static string CurrentWorldName => PlayerPrefs.GetString("CurrentWorld", "default_world");
    public static int CurrentSeed => PlayerPrefs.GetInt("CurrentSeed", 0);
    private static Database _itemDb;
    public static Database itemDb
    {
        get
        {
            if (_itemDb == null)
            {
                _itemDb = Resources.Load<Database>("ItemDatabase");
                if (_itemDb == null)
                {
                    Debug.LogError("Failed to load ItemDatabase from Resources!");
                }
            }
            return _itemDb;
        }
    }
    public static void SaveWorld()
    {
        WorldSaveData saveData = new WorldSaveData();

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        saveData.players = new PlayerSaveData[players.Length];
        saveData.seed = CurrentSeed;
        for (int i = 0; i < players.Length; i++)
        {
            saveData.players[i] = new PlayerSaveData(players[i]);
        }



        string json = JsonUtility.ToJson(saveData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + CurrentWorldName + ".json", json);
        Debug.Log(Application.persistentDataPath + "/" + CurrentWorldName + ".json");
    }

    public static void LoadWorld()
    {
        string path = Application.persistentDataPath + "/" + CurrentWorldName + ".json";
        if (!File.Exists(path))
        {
            SaveWorld();
        }
        else
        {
            string json = File.ReadAllText(path);
            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            foreach (PlayerSaveData playerData in data.players)
            {
                Player player = FindAnyObjectByType<Player>();
                LoadToPlayer(player, playerData);
            }
        }
    }
    public static void LoadToPlayer(Player player, PlayerSaveData data)
    {
        player.transform.position = data.position;
        player.SetCurrentHealth(data.health);


        for (int i = 0; i < data.inventory.Length; i++)
        {
            if (data.inventory[i].amount > 0)
            {
                Debug.Log(itemDb);
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

