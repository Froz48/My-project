using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class EscMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button saveAndExitButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        saveAndExitButton.onClick.AddListener(OnSaveAndExit);
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
            }
        }

        SceneManager.LoadScene("Menu");
    }
}