

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
    private ItemDatabase databaseItems;

    // public override void OnNetworkSpawn(){
    //     NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId ,sceneName, loadSceneMode)=>{Instantiate(playerPrefab)};
    // }
    private void Awake() {
        databaseItems = Resources.Load<ItemDatabase>("ItemDatabase");
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
            _gameObject.GetComponent<GroundItem>().setItem(databaseItems.GetItem(spawnItemId));
            _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
            _gameObject.GetComponent<NetworkObject>().Spawn();
        });

    }

}