
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("AbilityManager");
                    _instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(obj); // Чтобы не уничтожался при загрузке сцены
                }
            }
            return _instance;
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
    private void Awake()
    {
        // Защита от дублирования
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}