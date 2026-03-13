using UnityEngine;
using System.Collections;

public class VisionMaskController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Arrastra aquí el objeto VisionMask que es hijo del Player")]
    [SerializeField] private Transform maskTransform;

    [Header("Scale Settings")]
    [Tooltip("Velocidad a la que se encoge el círculo al recibir daño")]
    [SerializeField] private float shrinkSpeed = 5f;
    
    [Tooltip("Tamaño de la máscara según las vidas. Índice 0 = 0 vidas, Índice 1 = 1 vida, etc.")]
    [SerializeField] private float[] lifeScales = { 0f, 2f, 4f, 6f, 8f, 10f };

    private float targetScale;

    private void Start()
    {
        // Nos suscribimos al evento de daño
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerHit += UpdateMaskSize;
            
            // Forzamos el tamaño inicial al cargar la escena
            UpdateMaskSize();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerHit -= UpdateMaskSize;
        }
    }

    private void UpdateMaskSize()
    {
        if (maskTransform == null) return;

        // Preguntamos cuántas vidas tenemos actualmente
        int currentLives = GameManager.Instance.Lives;

        // Protegemos el índice por si el jugador llega a tener más de 5 vidas o menos de 0
        int index = Mathf.Clamp(currentLives, 0, lifeScales.Length - 1);
        
        // Obtenemos la escala objetivo para esa cantidad de vidas
        targetScale = lifeScales[index];

        // Detenemos cualquier animación previa e iniciamos la nueva
        StopAllCoroutines();
        StartCoroutine(ScaleMaskRoutine());
    }

    private IEnumerator ScaleMaskRoutine()
    {
        // Creamos el vector de escala (X, Y iguales para mantener el círculo)
        Vector3 targetVector = new Vector3(targetScale, targetScale, 1f);
        
        // Mientras la escala actual no sea casi igual a la objetivo...
        while (Vector3.Distance(maskTransform.localScale, targetVector) > 0.01f)
        {
            // Interpolar suavemente el tamaño
            maskTransform.localScale = Vector3.Lerp(maskTransform.localScale, targetVector, Time.deltaTime * shrinkSpeed);
            yield return null;
        }
        
        // Aseguramos el valor exacto al final
        maskTransform.localScale = targetVector;
    }
}