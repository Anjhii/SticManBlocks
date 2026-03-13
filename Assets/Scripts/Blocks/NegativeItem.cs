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
    private Transform playerTransform;
    private Vector3 originalScale; // ✅ Guardamos la escala original

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        halfWidth = col.bounds.extents.x;
        halfHeight = col.bounds.extents.y;

        float minLevelSpeed = LevelManager.Instance.GetMinObstacleSpeed();
        float maxLevelSpeed = LevelManager.Instance.GetMaxObstacleSpeed();
        currentSpeed = Random.Range(minLevelSpeed, maxLevelSpeed);

        float randomX = Random.value > 0.5f ? 1f : -1f; 
        float randomY = Random.value > 0.5f ? 1f : -1f; 
        
        Vector2 initialDirection = new Vector2(randomX, randomY).normalized;
        rb.linearVelocity = initialDirection * currentSpeed;

        // ✅ Buscamos al player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        // ✅ Guardamos la escala original del prefab
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // ✅ Voltea el sprite según si el player está a la izquierda o derecha
            if (direction.x < 0)
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            else
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);

            // ✅ Pequeño tilt hacia el player máximo 30°
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(angle, -30f, 30f);

            if (direction.x < 0)
                clampedAngle = Mathf.Clamp(-angle, -30f, 30f);

            transform.rotation = Quaternion.Euler(0f, 0f, clampedAngle);
        }
    }

    private void FixedUpdate()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        float boundRight  = cameraPos.x + camHalfWidth  - halfWidth;
        float boundLeft   = cameraPos.x - camHalfWidth  + halfWidth;
        float boundTop    = cameraPos.y + camHalfHeight  - halfHeight;
        float boundBottom = cameraPos.y - camHalfHeight  + halfHeight;

        Vector2 velocity = rb.linearVelocity;
        Vector2 position = rb.position;

        if (position.x >= boundRight && velocity.x > 0)
            velocity.x = -velocity.x;
        else if (position.x <= boundLeft && velocity.x < 0)
            velocity.x = -velocity.x;

        if (position.y >= boundTop && velocity.y > 0)
            velocity.y = -velocity.y;
        else if (position.y <= boundBottom && velocity.y < 0)
            velocity.y = -velocity.y;

        rb.linearVelocity = velocity.normalized * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.dano);

            if (GameManager.Instance.IsShieldActive)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.powerupVF);
                Destroy(gameObject);
                return; 
            }

            GameManager.Instance.RemoveScore(penaltyPoints);
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}