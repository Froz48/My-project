
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[CreateAssetMenu(fileName = "InterestGenerator", menuName = "InterestGenerator")]
public class InterestGenerator : ScriptableObject
{
    [SerializeField] List<GameObject> gameObjects;
    GameObject parentObject;

    public void GenerateChunk(Vector2Int chunk)
    {
        NativeArray<float> noiseValues = new NativeArray<float>(Config.CHUNK_SIZE * Config.CHUNK_SIZE, Allocator.TempJob);
        JGenerateChunkStructure jGenerateChunkCityNoise = new JGenerateChunkStructure
        {
            chunkCoord = chunk,
            results = noiseValues

        };
        JobHandle handle = jGenerateChunkCityNoise.Schedule();
        handle.Complete();
        ThreadQueuer.QueueMainThreadFunction(() => ApplyChunk(chunk, noiseValues));
    }

    void ApplyChunk(Vector2Int chunkCoord, NativeArray<float> noiseTemperatureValues)
    {
        if (parentObject == null) parentObject = GameObject.Find("InterestObjects");
        Vector2Int startWorldPosition = MapGen.GetWorldPosition(chunkCoord);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                int index = x + y * Config.CHUNK_SIZE;
                if (noiseTemperatureValues[index] <= -0.99)
                {
                    Vector3Int tilePosition = new Vector3Int(startWorldPosition.x + x, startWorldPosition.y + y, 0);
                    Instantiate(GetRandomGameObject(), tilePosition, Quaternion.identity, parentObject.transform);
                }
            }
        }
    }
    GameObject GetRandomGameObject()
    {
        return gameObjects[Random.Range(0, gameObjects.Count)];
    }
} 

public struct JGenerateChunkStructure : IJob{
    public NativeArray<float> results;
    public Vector2Int chunkCoord;
    public void Execute(){
        Vector2Int startWorldPosition = new Vector2Int(chunkCoord.x * Config.CHUNK_SIZE, chunkCoord.y * Config.CHUNK_SIZE);
        FastNoiseLite tempNoise = new FastNoiseLite();
        MapGen.SetNoiceParams(tempNoise, seed:1, frequency: 0.2f, cellularReturnType: FastNoiseLite.CellularReturnType.Distance2Div, 
            cellularDistanceFunction: FastNoiseLite.CellularDistanceFunction.EuclideanSq, cellulalJitter: 0.45f);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                Vector2Int currentWorldPosition = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);
                float tempNoiseValue = tempNoise.GetNoise(currentWorldPosition.x, currentWorldPosition.y);

                int index = x + y * Config.CHUNK_SIZE;
                results[index] = tempNoiseValue;
                if (tempNoiseValue <= -0.99f){
                    return;
                }
            }
        }
    }
}