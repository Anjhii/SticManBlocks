using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class NegativeItem : MonoBehaviour
{
    [Header("Penalty Settings")]
    [SerializeField] private int penaltyPoints = 15;
    
    private Rigidbody2D rb;
    private float halfWidth;
    private float halfHeight;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Mantenemos el objeto como Kinematic (Fantasma controlado matemáticamente)
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        // 1. Calculamos la "caja" de colisión del objeto para que el rebote sea en el borde exacto
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        halfWidth = col.bounds.extents.x;
        halfHeight = col.bounds.extents.y;

        // 2. Asignamos una velocidad aleatoria única para esta instancia
        float minLevelSpeed = LevelManager.Instance.GetMinObstacleSpeed();
        float maxLevelSpeed = LevelManager.Instance.GetMaxObstacleSpeed();
        currentSpeed = Random.Range(minLevelSpeed, maxLevelSpeed);


        // 3. Dirección inicial aleatoria (Zig-Zag puro en cualquier dirección)
        float randomX = Random.value > 0.5f ? 1f : -1f; 
        float randomY = Random.value > 0.5f ? 1f : -1f; 
        
        Vector2 initialDirection = new Vector2(randomX, randomY).normalized;
        rb.linearVelocity = initialDirection * currentSpeed;
    }

    private void FixedUpdate()
    {
        // LÓGICA DE REBOTE ESTILO DVD
        
        // Obtenemos la posición de la cámara en tiempo real (vital porque la cámara sube)
        Vector3 cameraPos = Camera.main.transform.position;
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        // Definimos los 4 muros matemáticos de la pantalla
        float boundRight = cameraPos.x + camHalfWidth - halfWidth;
        float boundLeft = cameraPos.x - camHalfWidth + halfWidth;
        float boundTop = cameraPos.y + camHalfHeight - halfHeight;
        float boundBottom = cameraPos.y - camHalfHeight + halfHeight;

        Vector2 velocity = rb.linearVelocity;
        Vector2 position = rb.position;

        // Rebote Horizontal (Izquierda / Derecha)
        if (position.x >= boundRight && velocity.x > 0)
        {
            velocity.x = -velocity.x;
        }
        else if (position.x <= boundLeft && velocity.x < 0)
        {
            velocity.x = -velocity.x;
        }

        // Rebote Vertical (Arriba / Abajo)
        if (position.y >= boundTop && velocity.y > 0)
        {
            velocity.y = -velocity.y;
        }
        else if (position.y <= boundBottom && velocity.y < 0)
        {
            velocity.y = -velocity.y;
        }

        // Aplicamos la nueva velocidad asegurando que la magnitud (rapidez) no se altere
        rb.linearVelocity = velocity.normalized * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ¿El escudo está activo?
            if (GameManager.Instance.IsShieldActive)
            {
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
        // (Opcional) Si en tu diseño actual el obstáculo nunca toca un collider "Ground"
        // porque rebota en la cámara, este else if no se ejecutará, pero dejarlo no afecta en nada.
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}