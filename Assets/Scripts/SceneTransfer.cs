
using UnityEngine.SceneManagement;

public class SceneTranfser{
    


    public void addScene(string sceneName){
        SceneManager.LoadSceneAsync(sceneName);
    }


}