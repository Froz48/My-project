using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    private const string WINDOW_MODE_KEY = "WindowModePreference";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAndApplySettings();
    }

    /// <param name="modeIndex">0: Fullscreen, 1: Borderless, 2: Windowed</param>
    public void SetWindowMode(int modeIndex)
    {
        FullScreenMode windowMode;

        switch (modeIndex)
        {
            case 0: // Fullscreen
                windowMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // Borderless
                windowMode = FullScreenMode.FullScreenWindow;
                break;
            case 2: // Windowed
            default:
                windowMode = FullScreenMode.Windowed;
                break;
        }

        Screen.fullScreenMode = windowMode;
        Debug.Log($"Window mode set to: {windowMode}");

        PlayerPrefs.SetInt(WINDOW_MODE_KEY, modeIndex);
        PlayerPrefs.Save();
    }

    private void LoadAndApplySettings()
    {
        int savedModeIndex = PlayerPrefs.GetInt(WINDOW_MODE_KEY, 0);
        
        SetWindowMode(savedModeIndex);
    }
    
    public int GetCurrentWindowModeIndex()
    {
        return PlayerPrefs.GetInt(WINDOW_MODE_KEY, 0);
    }
}