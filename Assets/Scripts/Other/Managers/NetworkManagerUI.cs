using System;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    [SerializeField] private GameObject groundItemPrefab;
    [SerializeField] private Button spawnItemButton;
    [SerializeField] private TMP_InputField textiid;
    private Database databaseItems;

    // public override void OnNetworkSpawn(){
    //     NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId ,sceneName, loadSceneMode)=>{Instantiate(playerPrefab)};
    // }
    private void Awake() {
        databaseItems = Resources.Load<Database>("ItemDatabase");

        spawnItemButton.onClick.AddListener((UnityEngine.Events.UnityAction)(() => {
            var _gameObject = Instantiate(groundItemPrefab, new Vector3(2,2,-1), quaternion.identity);

            Debug.Log(textiid.text.ToIntArray());
           int iid = Convert.ToInt32(textiid.text);
            _gameObject.GetComponent<GroundItem>().SetItemClientRpc(iid);
            _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().GetItem().sprite;
            _gameObject.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Spawned item with id = " + iid);
        }));

    }

}