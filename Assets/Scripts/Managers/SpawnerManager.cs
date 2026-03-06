using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject blockPrefab;
    
    [Header("Timing Settings")]
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float spawnY = 8f;
    
    [Header("Lane Settings")]
    [SerializeField] private int numberOfLanes = 5; 
    
    [Header("Difficulty Settings")]
    [Range(0f, 1f)] 
    [SerializeField] private float doubleSpawnChance = 0.3f; 
    
    [Header("Score Item Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float itemSpawnChance = 0.5f; // Probabilidad de que aparezca CUALQUIER item
    [SerializeField] private float itemOffsetY = 0.8f; 
    
    [Tooltip("El índice 0 será el normal, el índice 1 será el raro.")]
    [SerializeField] private GameObject[] scoreItemPrefabs; 
    
    [Header("Rarity Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float rareItemChance = 0.1f; // 10% de probabilidad de que el item sea el Raro
    
    [Header("Obstacles Settings (Negative)")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float obstacleSpawnInterval = 3f;

    private float[] lanePositionsX;
    private Transform playerTransform;

    private float lastSpawnedBlockX = 0f;

    private void Start()
    {
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
        
        // 2. Iniciamos el ciclo de los bloques normales
        InvokeRepeating(nameof(SpawnBlocks), 0.5f, spawnInterval);
        
        // 3. Iniciamos el ciclo independiente de los obstáculos negativos
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

    private void SpawnSingleBlock(int laneIndex)
    {
        float spawnX = lanePositionsX[laneIndex];
        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        lastSpawnedBlockX = spawnX;
        
        GameObject newBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

        // 1. Verificamos si este bloque tendrá algún item
        if (Random.value < itemSpawnChance && scoreItemPrefabs != null && scoreItemPrefabs.Length > 0)
        {
            int selectedIndex = 0; // Por defecto, seleccionamos el objeto común (índice 0)

            // 2. Verificamos si tenemos un segundo objeto configurado y si tenemos suerte para sacarlo
            if (scoreItemPrefabs.Length > 1 && Random.value < rareItemChance)
            {
                selectedIndex = 1; // ¡Ganamos la lotería! Seleccionamos el objeto raro (índice 1)
            }

            GameObject selectedPrefab = scoreItemPrefabs[selectedIndex];

            // 3. Instanciamos el item directamente como hijo del bloque (más eficiente)
            GameObject item = Instantiate(selectedPrefab, newBlock.transform);
            
            // 4. Posicionamos el item relativo a su padre (el bloque)
            item.transform.localPosition = new Vector3(0f, itemOffsetY, 0f);
        }
    }

    //METODO Para que el PlayerController sepa dónde nació el último bloque y así posicionarse correctamente al morir
    public float GetLastSpawnedBlockX()
    {
        return lastSpawnedBlockX;
    }
    
    // MÉTODO Para saber a qué altura nacen los bloques
    public float GetSpawnY()
    {
        return spawnY;
    }

    private void SpawnObstacle()
    {
        if (playerTransform == null || obstaclePrefab == null) return;

        int safeLaneIndex = 0;
        float playerX = playerTransform.position.x;

        // Buscamos un carril que esté lejos del jugador para no spawnearle encima
        // Intentaremos hasta 5 veces encontrar un carril libre
        for (int i = 0; i < 5; i++)
        {
            safeLaneIndex = Random.Range(0, numberOfLanes);
            float laneX = lanePositionsX[safeLaneIndex];

            // Si la distancia horizontal entre el carril y el jugador es mayor a 1 unidad, es seguro
            if (Mathf.Abs(laneX - playerX) > 1f)
            {
                break; 
            }
        }

        Vector2 spawnPos = new Vector2(lanePositionsX[safeLaneIndex], spawnY);
        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
    }
}