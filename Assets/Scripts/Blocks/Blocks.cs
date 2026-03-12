using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour
{
    [Header("Fall Settings")]
    
    private Rigidbody2D rb;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Convertimos el bloque en Kinematic para tener control total de la velocidad
        // y evitar que el peso del jugador lo hunda.
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        // Obligatorio para que un objeto Kinematic detecte colisiones con objetos Static (el suelo base)
        rb.useFullKinematicContacts = true; 
    }

    private void Start()
    {
        // Toma la velocidad parametrizada del nivel actual
        currentSpeed = LevelManager.Instance.GetBlockSpeed();
    }

    private void FixedUpdate()
    {
        // Aplicamos una velocidad constante hacia abajo (sin aceleración gravitacional)
        rb.linearVelocity = Vector2.down * currentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sigue destruyéndose al tocar el suelo base
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}