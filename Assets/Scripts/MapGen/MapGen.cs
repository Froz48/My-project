using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{ // https://auburn.github.io/FastNoiseLite/
  
    const int CHUNK_SIZE = Config.CHUNK_SIZE;
    private HashSet<Vector2Int> generatedChunks = new HashSet<Vector2Int>();
    FastNoiseLite TempNoise;
    FastNoiseLite RainNoise;
    public Tilemap tilemap;
    [SerializeField] private Dictionary<Biome, int> biomeStats = new Dictionary<Biome, int>();
    const float CHUNK_GEN_TRY_FREQUENCY = 10f;
    [SerializeField] List<Biome> biomes;
    [SerializeField] private Transform playerTransform;
    [SerializeField]
    public List<TileBase> tileCache = new List<TileBase>();
    private int RenderDistance = 5;
    public static void SetNoiceParams(FastNoiseLite noise, int seed, float frequency, 
            FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2,
            FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.FBm,
            int octaves = 2,
            float lacunarity = 1f,
            float gain = 0.5f,
            FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2,
            float amplitude = 5f
            )
    {
        //noise.SetRotationType3D(FastNoiseLite.RotationType3D.None);
        noise.SetSeed(seed);
        noise.SetFrequency(frequency);
        noise.SetNoiseType(noiseType);
        noise.SetFractalType(fractalType);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalGain(gain);
        noise.SetDomainWarpType(domainWarpType);
        noise.SetDomainWarpAmp(amplitude);
        
    }

    private void CreateNoises(){
        TempNoise = new FastNoiseLite();
        RainNoise = new FastNoiseLite();
        SetNoiceParams(TempNoise, seed:1, frequency: 0.01f);
        SetNoiceParams(RainNoise, seed:1, frequency: 0.001f);
    }

    public void Start(){
        tilemap = GameObject.FindObjectOfType<Tilemap>();
        InitializeStatistics();
        TestBiomes(0.05f);
        CreateNoises();
        GenerateTestSprites();
        StartCoroutine(MapGenerating());
        
    }

    void InitializeStatistics(){
       foreach (Biome biome in biomes){
            biomeStats.Add(biome, 0);
        }
    }
    IEnumerator MapGenerating(){
        while (true)
        {
            GenerateNearbyChunks(GetPlayerChunkCoordinates(), RenderDistance);
            foreach (var biome in biomeStats){
                Debug.Log(biome.Key.name + " " + biome.Value);
            }
            yield return new WaitForSeconds(CHUNK_GEN_TRY_FREQUENCY);
        }   
    }
    public void Update(){
        if (ThreadQueuer.mainThreadActions.Count > 0){
            Action action = ThreadQueuer.mainThreadActions[0];
            ThreadQueuer.mainThreadActions.RemoveAt(0);
            action.Invoke();
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

    private void GenerateTestSprites(){
        tileCache.Clear();
        for (int i = 0; i < 256; i++) {
        RuleTile tile = new RuleTile();
        
        
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

    private TileBase ChooseTileTest(float value){
        value += 1;
        value /= 2;
        value = value * 255;
        return tileCache[(int)Mathf.Round(value)];
    }

    private TileBase ChooseTile(float temperature, float rainfall){
        foreach (Biome biome in biomes){
            {
                if (temperature >= biome.temperatureLeftBorder && temperature <= biome.temperatureRightBorder 
                    &&
                    rainfall >= biome.rainfallLeftBorder && rainfall <= biome.rainfallRightBorder)
                {
                    biomeStats[biome]++;
                    return biome.tile;
                }
            }
        }
        Debug.Log("No biome found for: " + temperature + " " + rainfall);
        return biomes[0].tile;
    }

    // private TileBase ChooseTileNoiceTemperature(float temperature, float rainfall)
    // {
    //     //округлить до 1 знака после запятой
    //     temperature = Mathf.Round(temperature * 10f) / 10f;
    //     if (tileCache.ContainsKey(temperature))
    //         return tileCache[temperature];
            
    //     Color noiseColor = new Color(temperature, temperature, temperature, 1f);
    //     Tile newTile = ScriptableObject.CreateInstance<Tile>();
    //     newTile.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    //     newTile.color = noiseColor;
        
    //     tileCache[temperature] = newTile;
    //     return newTile;
    // }
    private  void GenerateChunkAsync(Vector2Int chunkCoord)
    {
        if (generatedChunks.Contains(chunkCoord))
            return;
        Thread thread = new Thread(() => GenerateChunkNoiseValuesChildThread(chunkCoord, TempNoise, RainNoise));
        thread.Start();
        generatedChunks.Add(chunkCoord);
    }
    private void GenerateChunkNoiseValuesChildThread(Vector2Int chunkCoord, FastNoiseLite tempNoise, FastNoiseLite rainNoise)
    {
        Vector2Int startWorldPosition = new Vector2Int(chunkCoord.x * CHUNK_SIZE, chunkCoord.y * CHUNK_SIZE);
        NativeArray<float> tempNoiseValues = new NativeArray<float>(CHUNK_SIZE * CHUNK_SIZE, Allocator.TempJob);
        NativeArray<float> rainNoiseValues = new NativeArray<float>(CHUNK_SIZE * CHUNK_SIZE, Allocator.TempJob);

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                Vector2Int currentWorldPosition = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);
                float tempNoiseValue = tempNoise.GetNoise(currentWorldPosition.x, currentWorldPosition.y);
                float rainNoiseValue = rainNoise.GetNoise(currentWorldPosition.x, currentWorldPosition.y);

                int index = x + y * CHUNK_SIZE;
                tempNoiseValues[index] = tempNoiseValue;
                rainNoiseValues[index] = rainNoiseValue;
            }
        }

        ThreadQueuer.QueueMainThreadFunction(() => ApplyChunkToTilemap(chunkCoord, tempNoiseValues, rainNoiseValues));
    }


        
    private void ApplyChunkToTilemap(Vector2Int chunkCoord, NativeArray<float> noiseTemperatureValues, NativeArray<float> noiseRainfallValues)
    {
        Vector2Int startWorldPosition = new Vector2Int(chunkCoord.x * CHUNK_SIZE, chunkCoord.y * CHUNK_SIZE);

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                Vector2Int currentWorldPosition = new Vector2Int(startWorldPosition.x + x, startWorldPosition.y + y);
    
                int index = x + y * CHUNK_SIZE;
                Vector3Int tilePosition = new Vector3Int(currentWorldPosition.x, currentWorldPosition.y, 0);

                // tilemap.SetTile(tilePosition, ChooseTile(noiseTemperatureValues[index], noiseRainfallValues[index]));
                tilemap.SetTile(tilePosition, ChooseTileTest(noiseTemperatureValues[index]));
            }
        }

        generatedChunks.Add(chunkCoord);
    }
    bool isBusy = false;
    public  void GenerateNearbyChunks(Vector2Int ChunkPos, int renderDistance) 
    {
        if (isBusy) return;
        Vector2Int vector2Int = new Vector2Int(ChunkPos.x, ChunkPos.y);
        vector2Int.x -= renderDistance;
        vector2Int.y -= renderDistance;


        for (int x = 0; x <= renderDistance*2; x++)
        {
            for (int y = 0; y <= renderDistance*2; y++)
            {
                int gx = vector2Int.x + x;
                int gy = vector2Int.y + y;
                //Debug.Log("GenNearbChunk " + gx + " " + gy);
                GenerateChunkAsync(new Vector2Int(gx, gy));
            }
        }
    }
    private Vector2Int GetPlayerChunkCoordinates(){
        Vector3 playerPosition = playerTransform.position;
        int playerChunkX = Mathf.FloorToInt(playerPosition.x / CHUNK_SIZE);
        int playerChunkY = Mathf.FloorToInt(playerPosition.y / CHUNK_SIZE);
        return new Vector2Int(playerChunkX, playerChunkY);
    }

    public Vector2Int GetChunkCoordinates(Vector3 worldPosition)
    {
        int chunkX = Mathf.FloorToInt(worldPosition.x / CHUNK_SIZE);
        int chunkY = Mathf.FloorToInt(worldPosition.y / CHUNK_SIZE);
        return new Vector2Int(chunkX, chunkY);
    }
}

