using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{ // https://auburn.github.io/FastNoiseLite/
#region Variables
    private HashSet<Vector2Int> generatedChunks = new HashSet<Vector2Int>();
    [SerializeField] Tilemap tilemap;
    [SerializeField] Dictionary<Biome, int> biomeStats = new Dictionary<Biome, int>();
    const float CHUNK_GEN_TRY_FREQUENCY = 10f;
    [SerializeField] List<Biome> biomes;
    [SerializeField] Transform playerTransform;
    private Queue<Vector2Int> chunksToGenerate = new Queue<Vector2Int>();
    [SerializeField] GameObject cityPrefab;
    float biomeCapacity;
    [SerializeField] public List<TileBase> tileCache = new List<TileBase>();
    private int RenderDistance = 1;
#endregion
#region Unity
    public void Start(){
        tilemap = GameObject.FindObjectOfType<Tilemap>();
        InitializeStatistics();
        UpdateBiomeCapacity();
        GenerateTestSprites();
        StartCoroutine(MapGenerating());

    }
    public void Update(){
        if (ThreadQueuer.mainThreadActions.Count > 0){
            Action action = ThreadQueuer.mainThreadActions[0];
            ThreadQueuer.mainThreadActions.RemoveAt(0);
            action.Invoke();
        }
        
        if (chunksToGenerate.Count > 0)
            GenerateChunk(chunksToGenerate.Dequeue());
    }
#endregion
#region Basic Methods
    public static void SetNoiceParams(FastNoiseLite noise, int seed, float frequency, // looks horrible
            FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Cellular,
            FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.FBm,
            int octaves = 1,
            float lacunarity = 1f,
            float gain = 0.0f,
            FastNoiseLite.CellularDistanceFunction cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Hybrid,
            FastNoiseLite.CellularReturnType cellularReturnType = FastNoiseLite.CellularReturnType.CellValue,
            float cellulalJitter = 1f,
            FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2,
            float amplitude = 5f
            )
    {
        noise.SetSeed(seed);
        noise.SetFrequency(frequency);
        noise.SetNoiseType(noiseType);
        noise.SetFractalType(fractalType);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalGain(gain);
        noise.SetDomainWarpType(domainWarpType);
        noise.SetDomainWarpAmp(amplitude);
        noise.SetCellularDistanceFunction(cellularDistanceFunction);
        noise.SetCellularReturnType(cellularReturnType);
        noise.SetCellularJitter(cellulalJitter); 
    }

    private void GenerateTestSprites(){
        tileCache.Clear();
        for (int i = 0; i < 256; i+=16) {
            RuleTile tile = RuleTile.CreateInstance<RuleTile>();
            Texture2D texture = new Texture2D(32, 32);
            Color color = new Color(i / 255f, i / 255f, i / 255f);
            Color[] pixels = new Color[32 * 32];
            for (int j = 0; j < pixels.Length; j++) {
                pixels[j] = color;
            }
            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            tile.m_DefaultSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            tileCache.Add(tile);
        }
    }
    private void UpdateBiomeCapacity(){
        biomeCapacity = 0;
        foreach (Biome biome in biomes){
            biomeCapacity += biome.biomeFrequency;
        }
        biomeCapacity /= 2; // because { -1 : 1 } noice vals = length 2
    }
    void InitializeStatistics(){
       foreach (Biome biome in biomes){
            biomeStats.Add(biome, 0);
        }
    }
    private void TestBiomes(float stepSize = 0.1f){
        Debug.Log("TestBiomes");
        for (float iT = 0; iT <= 1; iT += stepSize)
        {
            for (float iR = 0; iR <= 1; iR += stepSize)
            {
                bool isValidBiomeFound = false;
                foreach (Biome biome in biomes)
                {
                    if (iT > biome.temperatureLeftBorder && iT <= biome.temperatureRightBorder 
                    &&
                    iR > biome.rainfallLeftBorder && iR <= biome.rainfallRightBorder){
                        if (isValidBiomeFound){
                            Debug.Log("Multiple biomes found for: " + iT + " " + iR);
                        }
                        isValidBiomeFound = true;
                    }
                }
                if (!isValidBiomeFound){
                    Debug.Log("No biome found for: " + iT + " " + iR);
                }
            }
        }
    }
    private Vector2Int GetPlayerChunkCoordinates(){
        Vector3 playerPosition = playerTransform.position;
        int playerChunkX = Mathf.FloorToInt(playerPosition.x / Config.CHUNK_SIZE);
        int playerChunkY = Mathf.FloorToInt(playerPosition.y / Config.CHUNK_SIZE);
        return new Vector2Int(playerChunkX, playerChunkY);
    }

    public Vector2Int GetChunkCoordinates(Vector3 worldPosition)
    {
        int chunkX = Mathf.FloorToInt(worldPosition.x / Config.CHUNK_SIZE);
        int chunkY = Mathf.FloorToInt(worldPosition.y / Config.CHUNK_SIZE);
        return new Vector2Int(chunkX, chunkY);
    }
#endregion
#region MapGen
    IEnumerator MapGenerating(){
        while (true)
        {
            GenerateNearbyChunks(GetPlayerChunkCoordinates(), RenderDistance);
            yield return new WaitForSeconds(CHUNK_GEN_TRY_FREQUENCY);
        }   
    }
    private TileBase ChooseTileTest(float value){
        value += 1;
        value /= 2;
        value = value * 255;
        return tileCache[(int)Mathf.Round(value)];
    }
    private TileBase ChooseTile(float temperature){
        float index = -1; // -1 : 1 noicevals
        foreach (Biome biome in biomes){
            index += biome.biomeFrequency/biomeCapacity;
            if (temperature <= index)
            {
                return biome.tile;
            }
        }
        Debug.Log("havent found tile");
        return biomes[0].tile;
    }
    private void GenerateChunk(Vector2Int chunkCoords){
        GenerateChunkBiomes(chunkCoords);
        GenerateChunkCities(chunkCoords);
    }
    private void GenerateChunkBiomes(Vector2Int chunkCoords){
        NativeArray<float> noiseValues = new NativeArray<float>(Config.CHUNK_SIZE * Config.CHUNK_SIZE, Allocator.TempJob);
        JGenerateChunkNoise jGenerateChunkNoise = new JGenerateChunkNoise{
            chunkCoord = chunkCoords,
            results = noiseValues
        };
        JobHandle handle = jGenerateChunkNoise.Schedule();
        handle.Complete();
        ThreadQueuer.QueueMainThreadFunction( () => ApplyChunkBiomeToTilemap(chunkCoords, noiseValues));
    }

    private void GenerateChunkCities(Vector2Int chunkCoords){
        NativeArray<float> noiseValues = new NativeArray<float>(Config.CHUNK_SIZE * Config.CHUNK_SIZE, Allocator.TempJob);
        JGenerateChunkStructure jGenerateChunkCityNoise = new JGenerateChunkStructure{
            chunkCoord = chunkCoords,
            results = noiseValues
        };
        JobHandle handle = jGenerateChunkCityNoise.Schedule();
        handle.Complete();
        Debug.Log("GenerateChunkCities");
        ThreadQueuer.QueueMainThreadFunction( () => ApplyChunkCityToTilemap(chunkCoords, noiseValues));
    }

    private static Vector2Int GetWorldPosition(Vector2Int chunkCoord){
        return new Vector2Int(chunkCoord.x * Config.CHUNK_SIZE, chunkCoord.y * Config.CHUNK_SIZE);
    }
    private void ApplyChunkBiomeToTilemap(Vector2Int chunkCoord, NativeArray<float> noiseValues)
    {
        Vector2Int startWorldPosition = GetWorldPosition(chunkCoord);
        
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                int index = x + y * Config.CHUNK_SIZE;
                Vector3Int tilePosition = new Vector3Int(startWorldPosition.x+x, startWorldPosition.y+y, 0);
                tilemap.SetTile(tilePosition, ChooseTile(noiseValues[index]));
            }
        }
        noiseValues.Dispose();
    }

    private void ApplyChunkCityToTilemap(Vector2Int chunkCoord, NativeArray<float> noiseTemperatureValues){
        Vector2Int startWorldPosition = GetWorldPosition(chunkCoord);
        for (int x = 0; x < Config.CHUNK_SIZE; x++){
            for (int y = 0; y < Config.CHUNK_SIZE; y++){
                int index = x + y * Config.CHUNK_SIZE;
                if (noiseTemperatureValues[index] <= -0.99){    
                    Debug.Log("City generated at: " + startWorldPosition.x+x + " " + startWorldPosition.y+y);
                    Vector3Int tilePosition = new Vector3Int(startWorldPosition.x+x, startWorldPosition.y+y, 0);
                    Instantiate(cityPrefab, tilePosition, Quaternion.identity);
                }
            }
        }
    }
    public void GenerateNearbyChunks(Vector2Int ChunkPos, int renderDistance) 
    {
        Vector2Int startChunk = new Vector2Int(ChunkPos.x - renderDistance, ChunkPos.y - renderDistance);
        for (int x = 0; x <= renderDistance*2; x++)
        {
            for (int y = 0; y <= renderDistance*2; y++)
            {
                Vector2Int currentChunk = new Vector2Int(startChunk.x + x, startChunk.y + y);
                if (!generatedChunks.Contains(currentChunk)){
                    generatedChunks.Add(currentChunk);
                    chunksToGenerate.Enqueue(currentChunk);
                }
            }
        }
    }

#endregion
}

public struct JGenerateChunkNoise : IJob
{   
    public NativeArray<float> results;
    public Vector2Int chunkCoord;
    public void Execute(){
        Vector2Int startWorldPosition = new Vector2Int(chunkCoord.x * Config.CHUNK_SIZE, chunkCoord.y * Config.CHUNK_SIZE);
        FastNoiseLite tempNoise = new FastNoiseLite();
        MapGen.SetNoiceParams(tempNoise, seed:1, frequency: 0.02f);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                Vector2Int currentWorldPosition = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);
                float tempNoiseValue = tempNoise.GetNoise(currentWorldPosition.x, currentWorldPosition.y);

                int index = x + y * Config.CHUNK_SIZE;
                results[index] = tempNoiseValue;
            }
        }
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