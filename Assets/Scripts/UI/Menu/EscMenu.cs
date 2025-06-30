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
    [SerializeField] private Button showOptionsButton;
    [SerializeField] private GameObject optionsGO;
    [SerializeField] private TextMeshProUGUI joinCodeText;

       private bool isInitialized = false;

    private void OnEnable()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        if (isInitialized)
        {
            UpdateJoinCode();
            return;
        }

        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        saveAndExitButton.onClick.AddListener(OnSaveAndExit);
        showOptionsButton.onClick.AddListener(ShowOptions);
        
        UpdateJoinCode();

        isInitialized = true;
    }
    private void ShowOptions()
    {
        optionsGO.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateJoinCode()
    {
        RelayManager relayManager = FindObjectOfType<RelayManager>();
        if (relayManager != null)
        {
            relayManager.ShowCode(joinCodeText);
        }
    }

    private void OnResume()
    {
        gameObject.SetActive(false);
    }

private void OnSave()
{
    if (SaveManager.Instance != null)
    {
        SaveManager.Instance.RequestSaveWorldServerRpc(false); 
        Debug.Log("Save request sent to server.");
    }
}

private void OnSaveAndExit()
{
    if (SaveManager.Instance != null)
    {
        SaveManager.Instance.RequestSaveWorldServerRpc(true); 
    }
}

    private System.Collections.IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}