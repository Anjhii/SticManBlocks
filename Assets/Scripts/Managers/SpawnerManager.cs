using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance { get; private set; }

    private float lastSpawnedBlockY;

    [Header("Prefabs")]
    [SerializeField] private GameObject blockPrefab;
    
    [Header("Timing Settings")]
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float spawnY = 8f;
    [SerializeField] private float blockGapY = 2f;
    
    [Header("Lane Settings")]
    [SerializeField] private int numberOfLanes = 5; 
    
    [Header("Difficulty Settings")]
    [Range(0f, 1f)] 
    [SerializeField] private float doubleSpawnChance = 0.3f; 
    
    [Header("Score Item Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float itemSpawnChance = 0.5f;
    [SerializeField] private float itemOffsetY = 0.8f; 
    
    [Tooltip("El índice 0 será el normal, el índice 1 será el raro.")]
    [SerializeField] private GameObject[] scoreItemPrefabs; 
    
    [Header("Rarity Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float rareItemChance = 0.1f;
    
    [Header("Obstacles Settings (Negative)")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float obstacleSpawnInterval = 3f;

    private float[] lanePositionsX;
    private Transform playerTransform;
    private float lastSpawnedBlockX = 0f;

    private void Awake()
    {
        // Inicializamos el Singleton (Esta es la solución definitiva al NullReference)
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject); // Evita que existan dos Spawners al mismo tiempo
        }
    }

    private void Start()
    {
        
        lastSpawnedBlockY = spawnY;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("SpawnerManager: No se encontró ningún objeto con el tag 'Player'.");
        }

        CalculateLanes();
        float currentSpawnInterval = LevelManager.Instance.GetSpawnInterval();
        
        InvokeRepeating(nameof(SpawnBlocks), 0.5f, spawnInterval);
        InvokeRepeating(nameof(SpawnObstacle), 2f, obstacleSpawnInterval);
    }

    private void CalculateLanes()
    {
        lanePositionsX = new float[numberOfLanes];
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float laneWidth = screenWidth / numberOfLanes;
        float startX = -(screenWidth / 2f) + (laneWidth / 2f);

        for (int i = 0; i < numberOfLanes; i++)
        {
            lanePositionsX[i] = startX + (i * laneWidth);
        }
    }

    private void SpawnBlocks()
    {
        bool spawnDouble = Random.value < doubleSpawnChance;

        if (spawnDouble)
        {
            int lane1 = Random.Range(0, numberOfLanes);
            int lane2 = Random.Range(0, numberOfLanes);
            
            while (lane2 == lane1)
            {
                lane2 = Random.Range(0, numberOfLanes);
            }

            SpawnSingleBlock(lane1);
            SpawnSingleBlock(lane2);
        }
        else
        {
            int lane = Random.Range(0, numberOfLanes);
            SpawnSingleBlock(lane);
        }
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnBlocks));
        CancelInvoke(nameof(SpawnObstacle));
        Debug.Log("Límite de nivel alcanzado. Spawner apagado.");
    }

    private void SpawnSingleBlock(int laneIndex)
    {
        float spawnX = lanePositionsX[laneIndex];

        
        lastSpawnedBlockY += blockGapY;
        Vector2 spawnPos = new Vector2(spawnX, lastSpawnedBlockY);
        lastSpawnedBlockX = spawnX;

        GameObject newBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

        if (Random.value < itemSpawnChance && scoreItemPrefabs != null && scoreItemPrefabs.Length > 0)
        {
            int selectedIndex = 0;
            if (scoreItemPrefabs.Length > 1 && Random.value < rareItemChance)
                selectedIndex = 1;

            GameObject selectedPrefab = scoreItemPrefabs[selectedIndex];
            GameObject item = Instantiate(selectedPrefab, newBlock.transform);
            item.transform.localPosition = new Vector3(0f, itemOffsetY, 0f);
        }
    }

    public float GetLastSpawnedBlockX() => lastSpawnedBlockX;
    public float GetSpawnY() => spawnY;

    private void SpawnObstacle()
    {
        if (playerTransform == null || obstaclePrefab == null) return;

        int safeLaneIndex = 0;
        float playerX = playerTransform.position.x;

        for (int i = 0; i < 5; i++)
        {
            safeLaneIndex = Random.Range(0, numberOfLanes);
            float laneX = lanePositionsX[safeLaneIndex];
            if (Mathf.Abs(laneX - playerX) > 1f) break;
        }

    
        Vector2 spawnPos = new Vector2(lanePositionsX[safeLaneIndex], lastSpawnedBlockY);
        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
    }
}