using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{ // https://auburn.github.io/FastNoiseLite/
#region Variables
private Tilemap tilemap;
HashSet<Vector2Int> occupiedCoordinates = new HashSet<Vector2Int>();
[SerializeField] private BiomeGenerator biomeGenerator;
private bool isReadyToGenerate = false;
private bool isInitialized = false; 
[SerializeField] private Transform playerTransform;
[SerializeField] InterestGenerator interestGenerator;
[SerializeField] MicsGenerator micsGenerator;
private HashSet<Vector2Int> generatedChunks = new HashSet<Vector2Int>();
private Queue<Vector2Int> chunksToGenerate = new Queue<Vector2Int>();
private const float CHUNK_GEN_TRY_FREQUENCY = 5f;
private int RenderDistance = 3;
private int worldSeed;
public int GetWorldSeed() => worldSeed;

    #endregion

    #region Unity
    public void Start()
    {
        if (!GetComponent<NetworkObject>().IsOwner)
        {
            enabled = false;
            return;
        }


         StartCoroutine(FullInitializationRoutine());
    }
        private IEnumerator FullInitializationRoutine()
    {
        // --- Этап 1: Ждем Tilemap ---
        while (tilemap == null)
        {
            tilemap = FindObjectOfType<Tilemap>();
            if (tilemap == null)
            {
                // Если не нашли, ждем следующий кадр и пробуем снова
                yield return null; 
            }
        }

        // --- Этап 2: Ждем GameManager ---
        while (GameManager.Instance == null || !GameManager.Instance.IsSpawned)
        {
            yield return null; // Ждем следующий кадр
        }

        // --- Этап 3: Финальная инициализация ---
        worldSeed = GameManager.Instance.WorldSeed.Value;
        biomeGenerator.Initialize(tilemap, worldSeed);
        
        isReadyToGenerate = true;
        StartCoroutine(MapGenerating());
    }

    public void Update()
    {
        if (!isReadyToGenerate) return;

        // if (!isInitialized && GameManager.Instance != null && GameManager.Instance.IsSpawned)
        // {
        //     worldSeed = GameManager.Instance.WorldSeed.Value;
        //     isInitialized = true;
        //     biomeGenerator.Initialize(tilemap, worldSeed);
        //     StartCoroutine(MapGenerating());
        // }
        // if (!isInitialized) return;

        if (ThreadQueuer.mainThreadActions.Count > 0)
        {
            Action action = ThreadQueuer.mainThreadActions[0];
            ThreadQueuer.mainThreadActions.RemoveAt(0);
            action.Invoke();
        }

        if (chunksToGenerate.Count > 0)
            GenerateChunk(chunksToGenerate.Dequeue());
    }

#endregion

#region Basic Methods
public static void SetNoiceParams(FastNoiseLite noise, int seed, float frequency, // abomination
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

private Vector2Int GetPlayerChunkCoordinates(){
    Vector3 playerPosition = playerTransform.position;
    int playerChunkX = Mathf.FloorToInt(playerPosition.x / Config.CHUNK_SIZE);
    int playerChunkY = Mathf.FloorToInt(playerPosition.y / Config.CHUNK_SIZE);
    return new Vector2Int(playerChunkX, playerChunkY);
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

private void GenerateChunk(Vector2Int chunkCoords)
{
    
    biomeGenerator.GenerateChunkBiomes(chunkCoords, worldSeed);
    interestGenerator.GenerateChunk(chunkCoords, worldSeed, occupiedCoordinates);
    micsGenerator.GenerateChunk(chunkCoords, worldSeed, occupiedCoordinates);
}


public static Vector2Int GetWorldPosition(Vector2Int chunkCoord){
    return new Vector2Int(chunkCoord.x * Config.CHUNK_SIZE, chunkCoord.y * Config.CHUNK_SIZE);
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