
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public NetworkVariable<int> WorldSeed = new NetworkVariable<int>();
    public override void OnNetworkSpawn()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (IsServer)
        {
            WorldSeed.Value = PlayerPrefs.GetInt("CurrentSeed", 0); 
            Debug.Log($"Server has set the world seed to: {WorldSeed.Value}");
        }
    }
    public Coroutine StartCoroutineM(IEnumerator coroutine)
    {
        return base.StartCoroutine(coroutine);
    }
    public GameObject InstantiateM(GameObject prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }
}