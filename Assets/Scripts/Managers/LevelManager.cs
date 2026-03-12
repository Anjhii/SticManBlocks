using UnityEngine;

// 1. EL MOLDE DE PARAMETRIZACIÓN
[System.Serializable]
public struct LevelConfig
{
    public int numberOfScreens; // Altura objetivo (Ej: 1)
    public float blockFallSpeed; 
    public float minObstacleSpeed; 
    public float maxObstacleSpeed; 
    public float spawnInterval;
    public float timeLimit;
    public int startingLives; 
    public float futureScrollSpeed; // Preparado para la siguiente fase
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Configurations")]
    [Tooltip("Índice 0 = Nivel 1, Índice 1 = Nivel 2, etc.")]
    [SerializeField] private LevelConfig[] levels;
    
    [Header("Portal Settings")]
    [SerializeField] private GameObject portalPrefab;
    
    private LevelConfig currentConfig;
    private Transform playerTransform;
    private Rigidbody2D playerRb;
    
    // Variables de progreso
    private float startY;
    private float targetY;
    private float highestYReached;
    private bool portalSpawned = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody2D>();
            startY = playerTransform.position.y;
            highestYReached = startY;
        }

        // 2. CARGAMOS LA CONFIGURACIÓN DEL NIVEL ACTUAL
        int levelIndex = GameManager.Instance.CurrentLevel - 1; // Level 1 es índice 0
        
        // Protección anti-errores si no has creado suficientes niveles en el Inspector
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            currentConfig = levels[levelIndex];
        }
        else
        {
            Debug.LogWarning("Nivel no configurado, usando Nivel 1 por defecto.");
            currentConfig = levels[0];
        }

        //Debug.Log($"<color=cyan>[LEVEL MANAGER]</color> Iniciando Nivel {GameManager.Instance.CurrentLevel}.");
        //Debug.Log($"<color=yellow>Parámetros Cargados:</color> Vel. Bloques: {currentConfig.blockFallSpeed} | Vel. Obstáculos: {currentConfig.obstacleFallSpeed} | Intervalo Spawn: {currentConfig.spawnInterval}");
        //Debug.Log($"Tiempo Límite: {currentConfig.timeLimit} | Vidas Iniciales: {currentConfig.startingLives}, Número de Pantallas: {currentConfig.numberOfScreens}");


        // Le enviamos al GameManager las vidas y el tiempo límite de este nivel
        GameManager.Instance.InitializeLevelParameters(currentConfig.timeLimit, currentConfig.startingLives);
        float screenTopY = Camera.main.orthographicSize;
        
        if (currentConfig.numberOfScreens <= 1)
        {
            // Si es 1 pantalla, la meta es casi el tope superior de la cámara actual.
            // Restamos 1.5f para que la meta esté dentro del área visible y alcanzable.
            targetY = screenTopY - 1.5f; 
        }
        else
        {
            // Lógica intacta para cuando implementemos el WorldScroll (Fase 2)
            float screenHeight = screenTopY * 2f;
            targetY = startY + (currentConfig.numberOfScreens * screenHeight);
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Condición de derrota por caída
        // En Update(), cambia la condición por esta:
        float limiteDerrota = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;

        if (playerTransform.position.y < limiteDerrota)
        {
            GameManager.Instance.FallDeath();
        }
        // 4. LÓGICA DE PROGRESIÓN (Spawneo del Portal)
        if (!portalSpawned)
        {
            // Registramos la altura máxima alcanzada
            if (playerTransform.position.y > highestYReached)
            {
                highestYReached = playerTransform.position.y;
            }

            // Si llegamos a la altura objetivo, generamos el portal arriba del jugador
            if (highestYReached >= targetY)
            {
                SpawnPortal();
            }
        }
    }

    private void SpawnPortal()
    {
        portalSpawned = true;
        // El portal aparece un poco más arriba de la meta para que el jugador lo vea venir
        Vector2 portalPos = new Vector2(0f, targetY + 0.5f); 
        Instantiate(portalPrefab, portalPos, Quaternion.identity);
        Debug.Log("¡Portal Generado! Ve hacia él.");
    }

    // --- GETTERS PÚBLICOS PARA EL RESTO DEL SISTEMA ---
    public float GetBlockSpeed() => currentConfig.blockFallSpeed;
    public float GetMinObstacleSpeed() => currentConfig.minObstacleSpeed;
    public float GetMaxObstacleSpeed() => currentConfig.maxObstacleSpeed;
    public float GetSpawnInterval() => currentConfig.spawnInterval;
}