// GameManager.cs (Actualizado)
using UnityEngine;
using UnityEngine.SceneManagement;
using System; // Necesario para Action

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public int CurrentLevel { get; private set; } = 1;
    public int SelectedSkinId { get; private set; } = 0;
    public string LastGameOverReason { get; private set; } = "";
    public bool IsShieldActive { get; set; } = false;
    public bool HasPowerUp => hasPowerUp;

    [Header("Time Settings")]
    [SerializeField] private float maxTime = 60f;
    private float currentTime;
    private bool isGameActive = false;

    [Header("Skin Database")]
    [Tooltip("Arrastra aquí los prefabs visuales de tus skins en orden (0 = Default, 1 = Skin2, etc)")]
    [SerializeField] private GameObject[] skinPrefabs;

    // EVENTO: Permite que otros scripts escuchen si el PowerUp está disponible
    public event Action<bool> OnPowerUpAvailabilityChanged;
    public event Action OnPlayerHit;
    
    private bool hasPowerUp = false;
    private int lastScoreEvaluatedForLives = 0;

    private void Awake()
    {
        // 1. TRANSFORMACIÓN A SINGLETON PERSISTENTE
        if (Instance == null)
        {
            Instance = this;
            
            transform.SetParent(null); 
            
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

// Este método se dispara automáticamente cada vez que se carga UNA escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Solo iniciamos el temporizador y el HUD si estamos en la escena de juego real
        if (scene.name == "Gameplay")
        {
            currentTime = maxTime;
            isGameActive = true;
            
            // --- EL FIX DEL BUG: LIMPIEZA DE ESTADOS ZOMBIES ---
            hasPowerUp = false;
            IsShieldActive = false; // Obligamos a apagar cualquier escudo huérfano
            
            // Forzamos la actualización a los suscriptores (UI) para que el botón se apague
            OnPowerUpAvailabilityChanged?.Invoke(false); 
            // ---------------------------------------------------

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHUD(Lives, Score);
                UIManager.Instance.UpdateTime(currentTime);
            }
        }
    }

    private void Update()
    {
        // Solo descontamos tiempo si el juego está activo
        if (isGameActive)
        {
            currentTime -= Time.deltaTime; // Resta el tiempo transcurrido desde el último frame
            
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTime(currentTime);

            // Condición de derrota por tiempo
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                if (UIManager.Instance != null) UIManager.Instance.UpdateTime(currentTime);
                GameOver("¡Tiempo Agotado!");
            }
        }
    }

    // --- MÉTODOS DE NAVEGACIÓN Y FLUJO ---

    public void StartNewGame()
    {
        Score = 0;
        Lives = 3;
        CurrentLevel = 1;
        lastScoreEvaluatedForLives = 0;
        SceneManager.LoadScene("LevelTransition"); // Vamos a la pantalla de transición primero
    }

    public void LoadNextLevel()
    {
        CurrentLevel++;
        SceneManager.LoadScene("LevelTransition");
    }

    public void GoToMainMenu()
    {
        isGameActive = false;
        SceneManager.LoadScene("Menu");
    }

    //MÉTODOS DE GAMEPLAY (Con protección Null)
    public void AddScore(int points)
    {
        Score += points;
        UIManager.Instance.UpdateHUD(Lives, Score);
    }

    public void RemoveScore(int points)
    {
        Score -= points;
        if (Score < 0) Score = 0; // Evitar puntaje negativo
        UIManager.Instance.UpdateHUD(Lives, Score);

        OnPlayerHit?.Invoke();
    }

    public void LoseLife()
    {
        if (!isGameActive) return;

        Lives--;
        if (UIManager.Instance != null) UIManager.Instance.UpdateHUD(Lives, Score);
        OnPlayerHit?.Invoke(); // Esto hará que el jugador parpadee en rojo

        if (Lives <= 0)
        {
            GameOver("¡Te quedaste sin vidas!");
        }
        /*else
        {
            if (LevelManager.Instance != null) LevelManager.Instance.RespawnPlayerAtTop(); // Esto es para que el jugador reaparezca en la parte superior del bloque actual
        }*/
    }

    public void FallDeath()
    {
        if (!isGameActive) return;

        Lives = 0;
        if (UIManager.Instance != null) UIManager.Instance.UpdateHUD(Lives, Score);
        GameOver("¡Caíste al vacío!");
    }

    private void GameOver(string reason)
    {
        isGameActive = false; // Detiene el temporizador y bloquea puntajes
        LastGameOverReason = reason;
        
        SceneManager.LoadScene("GameOver");
        AudioManager.Instance.PlayMusic(AudioManager.Instance.gameOverMusic);
    }

    // Llamado cuando el jugador toca el item raro
    public void CollectPowerUp()
    {
        if (!isGameActive || hasPowerUp) return;
        hasPowerUp = true;
        OnPowerUpAvailabilityChanged?.Invoke(true); // Avisa a la UI
    }

    // Llamado por el PowerUpManager cuando se gasta
    public void ConsumePowerUp()
    {
        if (!isGameActive) return;
        hasPowerUp = false;
        OnPowerUpAvailabilityChanged?.Invoke(false); // Avisa a la UI
    }

    public void InitializeLevelParameters(float timeLimit, int startingLives)
    {
        // Solo sobrescribimos las vidas si estamos en el nivel 1 (para no borrar las vidas guardadas del nivel anterior)
        if (CurrentLevel == 1) 
        {
            Lives = startingLives;
        }
        
        currentTime = timeLimit;
        isGameActive = true;

        hasPowerUp = false;
        IsShieldActive = false; 
        OnPowerUpAvailabilityChanged?.Invoke(false);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHUD(Lives, Score);
            UIManager.Instance.UpdateTime(currentTime);
        }
    }

    public void CompleteLevel()
    {
        isGameActive = false; // Pausamos el tiempo y el juego
        
        int evaluatedPoints = Score - lastScoreEvaluatedForLives;
        int livesGained = evaluatedPoints / 100;
        
        if (livesGained > 0)
        {
            Lives += livesGained;
            
            // Límite estricto de 5 vidas máximo
            if (Lives > 5) Lives = 5; 

            // Actualizamos el checkpoint. (Ej: Si teníamos 250, guardamos 200 y sobran 50 para el próximo nivel)
            lastScoreEvaluatedForLives += livesGained * 100;
        }

        if (CurrentLevel >= 3) 
        {
            SceneManager.LoadScene("Victory");
            AudioManager.Instance.PlayMusic(AudioManager.Instance.winMusic);
        }
        else
        {
            CurrentLevel++;
            SceneManager.LoadScene("LevelTransition");
        }
    }

    public void SetSkin(int skinId)
    {
        SelectedSkinId = skinId;
        Debug.Log("Skin equipada: " + skinId);
    }

    public GameObject GetSelectedSkinPrefab()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0) return null;
        
        // Seguro anti-errores por si se pide una skin que no existe
        if (SelectedSkinId < 0 || SelectedSkinId >= skinPrefabs.Length) return skinPrefabs[0];
        
        return skinPrefabs[SelectedSkinId];
    }

    public int GetTotalSkins() 
    {
        return (skinPrefabs != null) ? skinPrefabs.Length : 0;
    }

    // Devuelve un prefab específico por su ID
    public GameObject GetSkinPrefab(int index)
    {
        if (skinPrefabs == null || index < 0 || index >= skinPrefabs.Length) return null;
        return skinPrefabs[index];
    }
}