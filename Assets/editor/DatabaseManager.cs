// DatabaseManager.cs (в папке Editor)
using UnityEngine;
using UnityEditor;

// Этот атрибут заставляет статический конструктор класса запускаться при загрузке редактора
// и перед входом в Play Mode.
[InitializeOnLoad]
public static class DatabaseManager
{
    // Статический конструктор
    static DatabaseManager()
    {
        // Подписываемся на событие изменения состояния Play Mode
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Нас интересует момент прямо перед входом в Play Mode
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            UpdateAllDatabasesFromConfig();
        }
    }

    [MenuItem("Tools/Update Databases From Config")] // Добавляем кнопку в меню для ручного вызова
    private static void UpdateAllDatabasesFromConfig()
    {
        Debug.Log("DatabaseManager: Updating databases specified in Config...");

        // Создаем массив с именами баз данных из вашего Config класса
        string[] databaseNames = new string[]
        {
            Config.DATABASE_NPC_NAME,
            Config.DATABASE_ITEM_NAME,
            Config.DATABASE_ABILITY_NAME,
            Config.DATABASE_DISTRICT_NAME
            // Добавьте сюда другие имена, если они появятся
        };

        int updatedCount = 0;

        // Проходим по каждому имени
        foreach (string dbName in databaseNames)
        {
            // Загружаем ассет из папки Resources
            Database database = Resources.Load<Database>(dbName);

            if (database != null)
            {
                // Вызываем публичный метод для обновления ID
                database.UpdateID();
                updatedCount++;
            }
            else
            {
                Debug.LogWarning($"DatabaseManager: Could not find database '{dbName}' in any Resources folder.");
            }
        }
        
        // Сохраняем все измененные ассеты на диск
        AssetDatabase.SaveAssets();
        Debug.Log($"DatabaseManager: Finished updating {updatedCount} out of {databaseNames.Length} specified databases.");
    }
}