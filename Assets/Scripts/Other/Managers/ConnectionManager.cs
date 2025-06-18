// Файл: ConnectionManager.cs
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private RelayManager relayManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void StartHost(bool useRelay)
    {
        if (useRelay)
        {
            Debug.Log("Starting host with Relay...");
            await relayManager.SetupRelay(); // Ждем, пока Relay настроится
        }
        else
        {
            Debug.Log("Starting host locally...");
            // Для локальной игры ничего дополнительно настраивать не нужно.
            // UnityTransport по умолчанию использует локальный IP.
        }
        if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started successfully. Loading Game scene...");
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public void StartClient(bool useRelay, string joinCode = "")
    {
        if (useRelay)
        {
            Debug.Log("Joining with Relay...");
            relayManager.JoinRelay(joinCode); // RelayManager сам запустит StartClient после настройки
        }
        else
        {
            Debug.Log("Joining locally...");
            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetConnectionData("127.0.0.1", 7777); // Стандартный порт
            NetworkManager.Singleton.StartClient();
        }
    }
}