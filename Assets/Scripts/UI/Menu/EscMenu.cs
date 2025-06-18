using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Relay;
using System.Threading.Tasks;

public class EscMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button saveAndExitButton;
    [SerializeField] private TextMeshProUGUI joinCodeText;

       private bool isInitialized = false;

    // Используем OnEnable, чтобы меню обновлялось каждый раз при открытии
    private void OnEnable()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        // Если инициализация уже была, просто обновляем код
        if (isInitialized)
        {
            UpdateJoinCode();
            return;
        }

        // Подписываемся на кнопки только один раз
        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        saveAndExitButton.onClick.AddListener(OnSaveAndExit);
        
        UpdateJoinCode();

        isInitialized = true;
    }

    private void UpdateJoinCode()
    {
        // Находим RelayManager и просим показать код
        RelayManager relayManager = FindObjectOfType<RelayManager>();
        if (relayManager != null)
        {
            relayManager.ShowCode(joinCodeText);
        }
    }

    private void OnResume()
    {
        // Просто выключаем панель меню
        gameObject.SetActive(false);
    }

private void OnSave()
{
    if (SaveManager.Instance != null)
    {
        SaveManager.Instance.RequestSaveWorldServerRpc(false); // false - не выходить
        Debug.Log("Save request sent to server.");
    }
}

private void OnSaveAndExit()
{
    if (SaveManager.Instance != null)
    {
        SaveManager.Instance.RequestSaveWorldServerRpc(true); // true - выйти после сохранения
    }
}

    private System.Collections.IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Логика выхода из сети и закрытия приложения
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            // Уничтожать NetworkManager не всегда безопасно, Shutdown обычно достаточно
            // Destroy(NetworkManager.Singleton.gameObject); 
        }
        
        Application.Quit();

        #if UNITY_EDITOR
        // Остановка Play Mode в редакторе
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}