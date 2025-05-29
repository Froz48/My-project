using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour
{
    public GameObject[] windows;

    public void HideAllWindows(){
        foreach (GameObject window in windows){
            window.SetActive(false);
        }
    }
    public void HideWindow(int windowIndex){
        windows[windowIndex].SetActive(false);
    }

    public void ShowWindow(int windowIndex){
        windows[windowIndex].SetActive(true);
    }

    public void ChangeWindowState(int windowIndex){
        windows[windowIndex].SetActive(!windows[windowIndex].activeSelf);
    }
    public void ShowOnlyWindow(int windowIndex){
        HideAllWindows();
        windows[windowIndex].SetActive(true);
    }
    public void QuitGame(){
        Application.Quit();
    }
    

}
