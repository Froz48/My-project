
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "InterestGenerator", menuName = "InterestGenerator")]
public class InterestGenerator : ScriptableObject
{
    [SerializeField] List<GameObject> gameObjects;
    private NetworkManager _networkManager;
    GameObject parentObject;

    public void GenerateChunk(Vector2Int chunk, int seed, HashSet<Vector2Int> occupiedCoordinates)
    {
        NativeArray<float> noiseValues = new NativeArray<float>(Config.CHUNK_SIZE * Config.CHUNK_SIZE, Allocator.TempJob);
        JGenerateChunkStructure jGenerateChunkCityNoise = new JGenerateChunkStructure
        {
            chunkCoord = chunk,
            seed = seed,
            results = noiseValues

        };
        JobHandle handle = jGenerateChunkCityNoise.Schedule();
        handle.Complete();
        ThreadQueuer.QueueMainThreadFunction(() => ApplyChunk(chunk, noiseValues, seed, occupiedCoordinates));
    }

    void ApplyChunk(Vector2Int chunkCoord, NativeArray<float> noiseTemperatureValues, int seed, HashSet<Vector2Int> occupiedCoordinates)
    {
        if (parentObject == null) parentObject = GameObject.Find("InterestObjects");

        System.Random random = new System.Random(seed + chunkCoord.x * 73856093 + chunkCoord.y * 19349663);

        Vector2Int startWorldPosition = MapGen.GetWorldPosition(chunkCoord);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                int index = x + y * Config.CHUNK_SIZE;
                if (noiseTemperatureValues[index] <= -0.99)
                {
                    Vector2Int spawnOrigin = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);

                    // --- НАЧАЛО НОВОЙ ЛОГИКИ ---

                    // 1. Выбираем, какой объект мы ХОТИМ здесь разместить
                    GameObject prefabToSpawn = GetRandomGameObject(random);
                    if (prefabToSpawn == null) continue;

                    // 2. Получаем его размер (footprint)
                    Vector2Int footprintSize = new Vector2Int(1, 1);
                    if (prefabToSpawn.TryGetComponent<ObjectFootprint>(out var footprint))
                    {
                        footprintSize = footprint.size;
                    }

                    // 3. ПРОВЕРКА: Свободна ли вся область под объект?
                    if (IsAreaFree(spawnOrigin, footprintSize, occupiedCoordinates))
                    {
                        // 4. РЕЗЕРВИРОВАНИЕ: Если свободна, занимаем все клетки
                        ReserveArea(spawnOrigin, footprintSize, occupiedCoordinates);

                        // 5. РАЗМЕЩЕНИЕ: Создаем объект
                        Instantiate(prefabToSpawn, (Vector3Int)spawnOrigin, Quaternion.identity, parentObject.transform);
                    }
                    // --- КОНЕЦ НОВОЙ ЛОГИКИ ---
                }
            }
        }
    }
    private bool IsAreaFree(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> occupiedCoordinates)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int currentTile = new Vector2Int(origin.x + i, origin.y + j);
                if (occupiedCoordinates.Contains(currentTile))
                {
                    return false; // Нашли занятую клетку, область не свободна
                }
            }
        }
        return true; // Все клетки свободны
    }
    private void ReserveArea(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> occupiedCoordinates)
{
    for (int i = 0; i < size.x; i++)
    {
        for (int j = 0; j < size.y; j++)
        {
            Vector2Int currentTile = new Vector2Int(origin.x + i, origin.y + j);
            occupiedCoordinates.Add(currentTile);
        }
    }
}
    GameObject GetRandomGameObject(System.Random rand)
    {
        if (gameObjects == null || gameObjects.Count == 0) return null;
        return gameObjects[rand.Next(0, gameObjects.Count)];
    }
} 

public struct JGenerateChunkStructure : IJob{
    public NativeArray<float> results;
    public Vector2Int chunkCoord;
    public int seed;
    public void Execute()
    {
        Vector2Int startWorldPosition = new Vector2Int(chunkCoord.x * Config.CHUNK_SIZE, chunkCoord.y * Config.CHUNK_SIZE);
        FastNoiseLite tempNoise = new FastNoiseLite();
        MapGen.SetNoiceParams(tempNoise, seed: seed, frequency: 0.2f, cellularReturnType: FastNoiseLite.CellularReturnType.Distance2Div,
            cellularDistanceFunction: FastNoiseLite.CellularDistanceFunction.EuclideanSq, cellulalJitter: 0.45f);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                Vector2Int currentWorldPosition = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);
                float tempNoiseValue = tempNoise.GetNoise(currentWorldPosition.x, currentWorldPosition.y);

                int index = x + y * Config.CHUNK_SIZE;
                results[index] = tempNoiseValue;
                if (tempNoiseValue <= -0.99f)
                {
                    return;
                }
            }
        }
    }
}