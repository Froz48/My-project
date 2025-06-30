using UnityEngine;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown windowModeDropdown;

    private void Start()
    {
        if (OptionsManager.Instance == null)
        {
            Debug.LogError("OptionsManager not found in the scene!");
            return;
        }

        windowModeDropdown.value = OptionsManager.Instance.GetCurrentWindowModeIndex();
        
        windowModeDropdown.onValueChanged.RemoveAllListeners();
        
        windowModeDropdown.onValueChanged.AddListener(OptionsManager.Instance.SetWindowMode);
    }

    private void OnDestroy()
    {
        if (windowModeDropdown != null && OptionsManager.Instance != null)
        {
            windowModeDropdown.onValueChanged.RemoveListener(OptionsManager.Instance.SetWindowMode);
        }
    }
}