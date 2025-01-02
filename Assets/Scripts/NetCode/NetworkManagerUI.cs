

using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] private GameObject groundItemPrefab;
    [SerializeField] private int spawnItemId;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button spawnItemButton;

    // public override void OnNetworkSpawn(){
    //     NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId ,sceneName, loadSceneMode)=>{Instantiate(playerPrefab)};
    // }
    private void Awake() {
        serverButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartServer();
        });

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
        spawnItemButton.onClick.AddListener(() => {
            var _gameObject = Instantiate(groundItemPrefab, new Vector3(2,2,-1), quaternion.identity);
            _gameObject.GetComponent<GroundItem>().setItem(ItemObject.CreateInstanceInitialized(spawnItemId));
            _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
            _gameObject.GetComponent<NetworkObject>().Spawn();
        });

    }

}