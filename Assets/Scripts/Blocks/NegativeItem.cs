using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class NegativeItem : MonoBehaviour
{
    [Header("Penalty Settings")]
    [SerializeField] private int penaltyPoints = 15;
    [SerializeField] private float fallSpeed = 3f; 
    
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Mantenemos el objeto como Kinematic para controlar su caída
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        // Caída constante independiente de la gravedad
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si chocó con el jugador
        if (collision.CompareTag("Player"))
        {
            // 1. Restamos los puntos
            GameManager.Instance.RemoveScore(penaltyPoints);
            
            // 2. Restamos la vida (Esta es la nueva línea)
            GameManager.Instance.LoseLife();
            
            // 3. Destruimos el obstáculo
            Destroy(gameObject);
        }
        // Si no tocó al jugador, pero llegó al límite inferior (Suelo), se destruye
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}