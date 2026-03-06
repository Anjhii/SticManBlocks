using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ScoreItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int pointsToGive = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo el jugador puede recoger esto
        if (collision.CompareTag("Player"))
        {
            // Añadimos los puntos usando nuestro GameManager centralizado
            GameManager.Instance.AddScore(pointsToGive);
            
            // Destruimos el objeto recolectable
            Destroy(gameObject);
        }
    }
}