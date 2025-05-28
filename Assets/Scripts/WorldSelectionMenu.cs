using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;

public class WorldSelectionMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform worldsContainer;
    [SerializeField] private GameObject worldButtonPrefab;
    [SerializeField] private TMP_InputField newWorldNameInput;
    [SerializeField] private Button createWorldButton;

    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        RefreshWorldsList();
        createWorldButton.onClick.AddListener(CreateNewWorld);
    }

    private void RefreshWorldsList()
    {
        // Очищаем текущий список
        foreach (Transform child in worldsContainer)
        {
            Destroy(child.gameObject);
        }

        // Получаем все сохранения
        var saveFiles = Directory.GetFiles(Application.persistentDataPath, "*.json")
                               .Select(Path.GetFileNameWithoutExtension)
                               .ToList();

        // Создаем кнопки для каждого мира
        foreach (var saveName in saveFiles)
        {
            GameObject buttonObj = Instantiate(worldButtonPrefab, worldsContainer);
            WorldButton worldButton = buttonObj.GetComponent<WorldButton>();
            worldButton.Initialize(saveName, () => LoadWorld(saveName));
        }
    }

    private void CreateNewWorld()
    {
        string worldName = newWorldNameInput.text.Trim();
        if (string.IsNullOrEmpty(worldName))
        {
            Debug.LogWarning("World name cannot be empty!");
            return;
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, worldName + ".json")))
        {
            Debug.LogWarning("World with this name already exists!");
            return;
        }

        // Создаем новое сохранение
        WorldSaveData initialData = new WorldSaveData
        {
            players = new PlayerData[0] // Пустой массив игроков для нового мира
        };

        string json = JsonUtility.ToJson(initialData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, worldName + ".json"), json);

        // Загружаем новый мир
        LoadWorld(worldName);
    }

    private void LoadWorld(string worldName)
    {
        // Сохраняем имя мира для использования в игре
        PlayerPrefs.SetString("CurrentWorld", worldName);

        // Загружаем игровую сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
}



public class WorldButton : MonoBehaviour
{
    [SerializeField] private TMP_Text worldNameText;
    [SerializeField] private Button button;

    public void Initialize(string name, System.Action onClickAction)
    {
        worldNameText.text = name;
        button.onClick.AddListener(() => onClickAction());
    }
}