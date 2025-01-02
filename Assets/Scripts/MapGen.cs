using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{ // https://auburn.github.io/FastNoiseLite/
  
    const int CHUNK_SIZE = Config.CHUNK_SIZE;
    private HashSet<Vector2Int> generatedChunks = new HashSet<Vector2Int>();
    FastNoiseLite noise;
    public Tilemap tilemap;
    public TileBase groundTile;
    [SerializeField] private Transform playerTransform;

    public void Start(){
        tilemap = GameObject.FindObjectOfType<Tilemap>();
        SetNoiceParams();
        StartCoroutine(MapGenerating());
    }

    IEnumerator MapGenerating(){
        while (true)
        {
            Debug.Log("Map generating" + GetPlayerChunkCoordinates());
            GenerateNearbyChunks(GetPlayerChunkCoordinates(), 3);
            yield return new WaitForSeconds(5f);
        }
    }
    public void SetNoiceParams(){
        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetRotationType3D(FastNoiseLite.RotationType3D.None);
        noise.SetSeed(1337);
        noise.SetFrequency(0.05f);
        noise.SetFractalType(FastNoiseLite.FractalType.None);
        noise.SetFractalOctaves(1);
    }
    private void GenerateChunk(Vector2Int chunkCoord)
    {
        Debug.Log("Generating chunk at " + chunkCoord);
        if (generatedChunks.Contains(chunkCoord))
            return;

        int startX = chunkCoord.x * CHUNK_SIZE;
        int startY = chunkCoord.y * CHUNK_SIZE;

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                int worldX = startX + x;
                int worldY = startY + y;
                
                float noiseValue = noise.GetNoise(worldX, worldY);
                Vector3Int tilePosition = new Vector3Int(worldX, worldY, 0);

                tilemap.SetTile(tilePosition, groundTile);
                Color tileColor = new Color(noiseValue, noiseValue, noiseValue, 1f);
                tilemap.SetColor(tilePosition, tileColor);
            }
        }

        generatedChunks.Add(chunkCoord);
    }

    private async Task GenerateChunkAsync(Vector2Int chunkCoord)
    {
        var noiseValues = new NativeArray<float>(CHUNK_SIZE * CHUNK_SIZE, Allocator.TempJob);
        try
        {
        var job = new GenerateChunkJob
        {
            chunkCoord = chunkCoord,
            noiseValues = noiseValues,
            frequency = 0.05f,
            seed = 1337
        };
        var handle = job.Schedule();
        handle.Complete();
        await Task.Run(() => handle.Complete());

        // Apply results to tilemap on main thread
        ApplyChunkToTilemap(chunkCoord.x, chunkCoord.y, noiseValues);
        }
        finally{
            if (noiseValues.IsCreated)
            noiseValues.Dispose();
        }
    }
    private void ApplyChunkToTilemap(int chunkX, int chunkY, NativeArray<float> noiseValues)
    {
        Vector2Int chunkCoord = new Vector2Int(chunkX, chunkY);
        if (generatedChunks.Contains(chunkCoord))
            return;

        int startX = chunkX * CHUNK_SIZE;
        int startY = chunkY * CHUNK_SIZE;

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                int worldX = startX + x;
                int worldY = startY + y;
                int index = x * CHUNK_SIZE + y;
                
                Vector3Int tilePosition = new Vector3Int(worldX, worldY, 0);
                float noiseValue = noiseValues[index];

                tilemap.SetTile(tilePosition, groundTile);
                tilemap.SetColor(tilePosition, new Color(noiseValue, noiseValue, noiseValue, 1f));
            }
        }

        generatedChunks.Add(chunkCoord);
    }
    public async void GenerateNearbyChunks(Vector2Int ChunkPos, int renderDistance) // please be copy
    {
        Vector2Int vector2Int = new Vector2Int(ChunkPos.x, ChunkPos.y);
        vector2Int.x -= renderDistance;
        vector2Int.y -= renderDistance;


        for (int x = 0; x <= renderDistance*2; x++)
        {
            for (int y = 0; y <= renderDistance*2; y++)
            {
                int gx = vector2Int.x + x;
                int gy = vector2Int.y + y;
                Debug.Log("GenNearbChunk " + gx + " " + gy);
                await GenerateChunkAsync(new Vector2Int(gx, gy));
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

public struct GenerateChunkJob : IJob{
    public Vector2Int chunkCoord;
    public NativeArray<float> noiseValues;
    public float frequency;
    public int seed;
    public void Execute(){
        var noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetRotationType3D(FastNoiseLite.RotationType3D.None);
        noise.SetSeed(seed);
        noise.SetFrequency(frequency);
        noise.SetFractalType(FastNoiseLite.FractalType.None);
        noise.SetFractalOctaves(1);
        for (int x = 0; x < Config.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Config.CHUNK_SIZE; y++)
            {
                int index = x * Config.CHUNK_SIZE + y;
                int worldX = chunkCoord.x * Config.CHUNK_SIZE + x;
                int worldY = chunkCoord.y * Config.CHUNK_SIZE + y;
                noiseValues[index] = noise.GetNoise(worldX, worldY);
            }
        }   
    }
}
