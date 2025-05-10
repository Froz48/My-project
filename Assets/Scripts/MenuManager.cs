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
    public void ClientGame(){
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoadedClient;
    }
    public void HostGame(){
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoadedHost;
    }
    private void OnGameSceneLoadedClient(Scene scene, LoadSceneMode mode){
        if (scene.name == "Game")
        {
            // Отписываемся от события, чтобы не вызывать метод лишний раз
            SceneManager.sceneLoaded -= OnGameSceneLoadedClient;

            // Запускаем клиент
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
    }

    private void OnGameSceneLoadedHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // Отписываемся от события, чтобы не вызывать метод лишний раз
            SceneManager.sceneLoaded -= OnGameSceneLoadedHost;
            
            // Запускаем сервер
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
    }

}
