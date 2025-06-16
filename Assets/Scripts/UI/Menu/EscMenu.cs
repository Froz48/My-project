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
    [SerializeField] private TextMeshProUGUI joinCode;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        saveAndExitButton.onClick.AddListener(OnSaveAndExit);
        FindObjectOfType<RelayManager>().ShowCode(joinCode);
    }
    private void OnResume()
    {
        gameObject.SetActive(false);
    }

    private void OnSave()
    {
        SaveManager.SaveWorld();
    }

    private void OnSaveAndExit()
    {
        SaveManager.SaveWorld();


        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
        Application.Quit();
        // SceneManager.LoadScene("Menu");
    }
}