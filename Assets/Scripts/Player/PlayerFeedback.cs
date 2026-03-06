using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerFeedback : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Guardamos su color base (ej. Azul)
    }

    private void Start()
    {
        // Nos suscribimos al evento del GameManager
        GameManager.Instance.OnPlayerHit += TriggerDamageFlash;
    }

    private void OnDestroy()
    {
        // Limpiamos la suscripción al destruir el objeto
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerHit -= TriggerDamageFlash;
    }

    private void TriggerDamageFlash()
    {
        // Detenemos cualquier parpadeo anterior para que no se sobrepongan
        StopAllCoroutines(); 
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Cambiamos al color de daño
        spriteRenderer.color = damageColor;
        
        // Esperamos una fracción de segundo
        yield return new WaitForSeconds(flashDuration);
        
        // Volvemos al color original
        spriteRenderer.color = originalColor;
    }
}