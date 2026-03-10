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
        if (collision.CompareTag("Player"))
        {
            // ¿El escudo está activo?
            if (GameManager.Instance.IsShieldActive)
            {
                // El escudo nos protege. Se destruye el obstáculo y NO restamos vidas.
                Debug.Log("¡Escudo desvió el daño!");
                Destroy(gameObject);
                return; 
            }

            // 1. Restamos los puntos
            GameManager.Instance.RemoveScore(penaltyPoints);
            // 2. Restamos la vida
            GameManager.Instance.LoseLife();
            // 3. Destruimos el obstáculo
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}