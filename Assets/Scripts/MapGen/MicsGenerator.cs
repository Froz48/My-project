
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[CreateAssetMenu(fileName = "MicsGenerator", menuName = "MicsGenerator")]
public class MicsGenerator : ScriptableObject
{
    [SerializeField] List<GameObject> gameObjects;
    [SerializeField] GameObject parentObject;

    public void GenerateChunk(Vector2Int chunk, int seed, HashSet<Vector2Int> occupiedCoordinates)
    {
        NativeArray<float> noiseValues = new NativeArray<float>(Config.CHUNK_SIZE * Config.CHUNK_SIZE, Allocator.TempJob);
        JGenerateChunkMisc jGenerateChunkCityNoise = new JGenerateChunkMisc
        {
            chunkCoord = chunk,
            results = noiseValues,
            seed = seed

        };
        JobHandle handle = jGenerateChunkCityNoise.Schedule();
        handle.Complete();
        ThreadQueuer.QueueMainThreadFunction(() => ApplyChunk(chunk, noiseValues, seed, occupiedCoordinates));
    }

    void ApplyChunk(Vector2Int chunkCoord, NativeArray<float> noiseTemperatureValues, int seed, HashSet<Vector2Int> occupiedCoordinates)
    {
        if (parentObject == null) parentObject = GameObject.Find("MiscObjects");
        System.Random random = new System.Random(seed + chunkCoord.x * 83492791 + chunkCoord.y * 13974653);

        Vector2Int startWorldPosition = MapGen.GetWorldPosition(chunkCoord);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                int index = x + y * Config.CHUNK_SIZE;
                if (noiseTemperatureValues[index] <= -0.99)
                {
                    Vector2Int worldPos = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);

                    if (occupiedCoordinates.Contains(worldPos))
                    {
                        continue;
                    }

                    occupiedCoordinates.Add(worldPos);

                    Instantiate(GetRandomGameObject(random), (Vector3Int)worldPos, Quaternion.identity, parentObject.transform);
                }
            }
        }
    }
    GameObject GetRandomGameObject(System.Random rand)
    {
        if (gameObjects == null || gameObjects.Count == 0) return null;
        return gameObjects[rand.Next(0, gameObjects.Count)];
    }
} 

public struct JGenerateChunkMisc : IJob{
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
                // if (tempNoiseValue <= -0.99f){
                //     return;
                // }
            }
        }
    }
}