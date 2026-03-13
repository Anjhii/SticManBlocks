using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    [Header("Shield References")]
    [Tooltip("Arrastra aquí el objeto hijo del Player que tiene el Sprite del escudo")]
    [SerializeField] private GameObject shieldVisual; 

    [Header("Scaling Settings")]
    [SerializeField] private float minDuration = 2f; // Duración para un swipe muy corto
    [SerializeField] private float maxDuration = 6f; // Duración para un swipe largo
    
    [SerializeField] private float minScale = 2.0f;    // Tamaño normal
    [SerializeField] private float maxScale = 4.0f;    // Escudo gigante

    private void Start()
    {
        // Aseguramos que el escudo empiece apagado
        if (shieldVisual != null) shieldVisual.SetActive(false);
        
        InputManager.Instance.OnSwipeUp += TryActivateShield;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnSwipeUp -= TryActivateShield;
    }

    private void TryActivateShield(float swipeMagnitude)
    {
        // Verificamos si tenemos un power up recolectado y no estamos usándolo ya
        if (GameManager.Instance.HasPowerUp && !GameManager.Instance.IsShieldActive)
        {
            GameManager.Instance.ConsumePowerUp(); // Gastamos el power up en el UI
            StartCoroutine(ShieldRoutine(swipeMagnitude));
        }
    }

    private IEnumerator ShieldRoutine(float swipeMagnitude)
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.powerupVF);
        GameManager.Instance.IsShieldActive = true;
        shieldVisual.SetActive(true);

        // Lógica de escalado: Lerp mezcla entre Min y Max basándose en el porcentaje (0.0 a 1.0)
        float currentDuration = Mathf.Lerp(minDuration, maxDuration, swipeMagnitude);
        float currentScale = Mathf.Lerp(minScale, maxScale, swipeMagnitude);

        // Aplicamos el tamaño visual
        shieldVisual.transform.localScale = new Vector3(currentScale, currentScale, 1f);

        float timer = currentDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdatePowerUpTimer(timer);
            }

            yield return null; // Esperamos al siguiente frame
        }
        
        if (UIManager.Instance != null) UIManager.Instance.UpdatePowerUpTimer(0f);

        // Apagamos el escudo
        shieldVisual.SetActive(false);
        GameManager.Instance.IsShieldActive = false;

        int lvl = GameManager.Instance.CurrentLevel;
        if (AudioManager.Instance != null)
        {
            if (lvl == 1) AudioManager.Instance.PlayMusic(AudioManager.Instance.level1VF);
            else if (lvl == 2) AudioManager.Instance.PlayMusic(AudioManager.Instance.level2VF);
            else AudioManager.Instance.PlayMusic(AudioManager.Instance.level3VF);
        }
    }
}