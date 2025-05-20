
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BiomeGenerator", menuName = "World/BiomeGenerator")]
public class BiomeGenerator : ScriptableObject
{
    [SerializeField] private List<Biome> biomes;
    private Tilemap tilemap;
    private float biomeCapacity;
    public void Initialize(Tilemap targetTilemap)
    {
        tilemap = targetTilemap;
        CalculateBiomeCapacity();
    }
    private void CalculateBiomeCapacity()
    {
        biomeCapacity = 0;
        foreach (var biome in biomes)
        {
            biomeCapacity += biome.biomeFrequency;
        }
        biomeCapacity /= 2f; // bcs [-1, 1]
    }
    public void GenerateChunkBiomes(Vector2Int chunkCoords)
    {
        NativeArray<float> noiseValues = new NativeArray<float>(
            Config.CHUNK_SIZE * Config.CHUNK_SIZE,
            Allocator.TempJob
        );

        new JGenerateBiomeNoise
        {
            chunkCoord = chunkCoords,
            results = noiseValues
        }.Schedule().Complete();

        ThreadQueuer.QueueMainThreadFunction(() =>
            ApplyBiomesToTilemap(chunkCoords, noiseValues));
    }
    private void ApplyBiomesToTilemap(Vector2Int chunkCoord, NativeArray<float> noiseValues)
    {
        Vector2Int startWorldPos = MapGen.GetWorldPosition(chunkCoord);

        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                Vector3Int tilePos = new Vector3Int(
                    startWorldPos.x + x,
                    startWorldPos.y + y,
                    0
                );

                tilemap.SetTile(tilePos, GetBiomeTile(noiseValues[x + y * Config.CHUNK_SIZE]));
            }
        }

        noiseValues.Dispose();
    }
    private TileBase GetBiomeTile(float temperatureValue)
    {
        float threshold = -1f; // bcs [-1, 1]

        foreach (var biome in biomes)
        {
            threshold += biome.biomeFrequency / biomeCapacity;
            if (temperatureValue <= threshold)
            {
                return biome.tile;
            }
        }

        Debug.LogWarning("No suitable biome found, using default");
        return biomes[0].tile;
    }
}
public struct JGenerateBiomeNoise : IJob
{
    public Vector2Int chunkCoord;
    public NativeArray<float> results;

    public void Execute()
    {
        FastNoiseLite noise = new FastNoiseLite();
        MapGen.SetNoiceParams(noise, seed: 1, frequency: 0.02f);
        Vector2Int startPos = new Vector2Int(
            chunkCoord.x * Config.CHUNK_SIZE,
            chunkCoord.y * Config.CHUNK_SIZE
        );

        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                results[x + y * Config.CHUNK_SIZE] = noise.GetNoise(
                    startPos.x + x,
                    startPos.y + y
                );
            }
        }
    }
}

