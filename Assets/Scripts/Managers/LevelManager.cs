using UnityEngine;

[System.Serializable]
public struct LevelConfig
{
    public int numberOfScreens;
    public float blockFallSpeed; 
    public float minObstacleSpeed; 
    public float maxObstacleSpeed; 
    public float spawnInterval;
    public float timeLimit;
    public int startingLives; 
    public float darknessOpacity;

    [Header("Visuales del Nivel")]
    public GameObject backgroundPrefab;
    public GameObject blockPrefab;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Configurations")]
    [Tooltip("Índice 0 = Nivel 1, Índice 1 = Nivel 2, etc.")]
    [SerializeField] private LevelConfig[] levels;
    
    [Header("Portal Settings")]
    [SerializeField] private LevelPortal portalActivator;

    [Header("Visual Effects")]
    [Tooltip("Arrastra aquí tu objeto DarknessOverlay")]
    [SerializeField] private SpriteRenderer darknessOverlay;

    [Header("Contenedores")]
    [SerializeField] private Transform backgroundContainer; // ✅ Arrastra el BackgroundContainer aquí

    private LevelConfig currentConfig;
    private Transform playerTransform;
    private Rigidbody2D playerRb;
    
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

        int levelIndex = GameManager.Instance.CurrentLevel - 1;
        
        if (levelIndex >= 0 && levelIndex < levels.Length)
            currentConfig = levels[levelIndex];
        else
        {
            Debug.LogWarning("Nivel no configurado, usando Nivel 1 por defecto.");
            currentConfig = levels[0];
        }

        if (darknessOverlay != null)
        {
            Color overlayColor = darknessOverlay.color;
            overlayColor.a = currentConfig.darknessOpacity;
            darknessOverlay.color = overlayColor;
        }

        GameManager.Instance.InitializeLevelParameters(currentConfig.timeLimit, currentConfig.startingLives);
        float screenTopY = Camera.main.orthographicSize;
        
        if (currentConfig.numberOfScreens <= 1)
            targetY = screenTopY - 1.5f; 
        else
        {
            float screenHeight = screenTopY * 2f;
            targetY = startY + (currentConfig.numberOfScreens * screenHeight);
        }

        int level = GameManager.Instance.CurrentLevel;
        if (level == 1) AudioManager.Instance.PlayMusic(AudioManager.Instance.level1VF);
        else if (level == 2) AudioManager.Instance.PlayMusic(AudioManager.Instance.level2VF);
        else if (level == 3) AudioManager.Instance.PlayMusic(AudioManager.Instance.level3VF);

        // ✅ Instancia el fondo del nivel actual
        if (currentConfig.backgroundPrefab != null && backgroundContainer != null)
        {
            foreach (Transform child in backgroundContainer)
                Destroy(child.gameObject);

            Instantiate(currentConfig.backgroundPrefab, backgroundContainer);
        }
        else
        {
            Debug.LogWarning("Background Prefab o BackgroundContainer no asignado.");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float limiteDerrota = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;

        if (playerTransform.position.y < limiteDerrota)
            GameManager.Instance.FallDeath();

        if (!portalSpawned)
        {
            if (playerTransform.position.y > highestYReached)
                highestYReached = playerTransform.position.y;

            if (highestYReached >= targetY)
                SpawnPortal();
        }
    }

    private void SpawnPortal()
    {
        portalSpawned = true;

        if (portalActivator != null)
        {
            portalActivator.EnableCollider();
            Debug.Log("¡Portal habilitado!");
        }
    }

    public float GetBlockSpeed() => currentConfig.blockFallSpeed;
    public float GetMinObstacleSpeed() => currentConfig.minObstacleSpeed;
    public float GetMaxObstacleSpeed() => currentConfig.maxObstacleSpeed;
    public float GetSpawnInterval() => currentConfig.spawnInterval;
    public GameObject GetBlockPrefab() => currentConfig.blockPrefab;
    public GameObject GetBackgroundPrefab() => currentConfig.backgroundPrefab;
}