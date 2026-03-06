using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Power Up UI")]
    [SerializeField] private Button powerUpButton; // Arrastra tu botón aquí en el Inspector

    private Image powerUpButtonImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Obtenemos el componente de imagen del botón para alterar su color
        if (powerUpButton != null)
        {
            powerUpButtonImage = powerUpButton.GetComponent<Image>();
        }
        
        // Nos suscribimos al evento
        GameManager.Instance.OnPowerUpAvailabilityChanged += TogglePowerUpButton;
        
        // Forzamos el estado inicial: Apagado y casi invisible
        TogglePowerUpButton(false); 
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPowerUpAvailabilityChanged -= TogglePowerUpButton;
    }

    public void UpdateHUD(int lives, int score)
    {
        livesText.text = $"Lives: {lives}";
        scoreText.text = $"Score: {score}";
    }

    public void UpdateTime(float timeRemaining)
    {
        // Formateamos el tiempo para que no muestre decimales feos.
        // Mathf.CeilToInt redondea hacia arriba, así 0.1 segundos se muestra como "1"
        int seconds = Mathf.CeilToInt(timeRemaining);
        timeText.text = $"Time: {seconds}s";
    }

    private void TogglePowerUpButton(bool isAvailable)
    {
        if (powerUpButton == null) return;

        // 1. Activa o desactiva la capacidad de hacer clic
        powerUpButton.interactable = isAvailable;

        // 2. Cambia la opacidad visual (Brillo)
        if (powerUpButtonImage != null)
        {
            Color colorActivo = powerUpButtonImage.color;
            
            // Si está disponible, alpha = 1 (100% visible). Si no, alpha = 0.2 (20% visible)
            colorActivo.a = isAvailable ? 1f : 0.2f; 
            
            powerUpButtonImage.color = colorActivo;
        }
    }
}