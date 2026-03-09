using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 18f; 
    [SerializeField] private float fallMultiplier = 3.5f; 
    
    // NUEVO: Filtro para ignorar el ruido del acelerómetro
    [Header("Input Filtering")]
    [Tooltip("Valores por debajo de este número se considerarán como 0 (dispositivo quieto).")]
    [SerializeField] private float deadZone = 0.05f; 

    [Header("Double Jump Settings")]
    [SerializeField] private int maxJumps = 2; // 1 salto normal + 1 en el aire
    private int jumpsRemaining;

    [Header("Collision Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Animator animator;
    
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private float screenBoundX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
        float playerHalfWidth = col.bounds.extents.x;
        screenBoundX = (Camera.main.orthographicSize * Camera.main.aspect) - playerHalfWidth;
    }

    private void Update()
    {
        // 1. Verificación del suelo y Animación
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        if (animator != null) 
            animator.SetBool("EnSuelo", isGrounded);

        // 2. Recargar saltos
        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            jumpsRemaining = maxJumps;
        }

        // 3. Lógica del Doble Salto limpio
        if (InputManager.Instance.WasJumpPressed)
        {
            if (jumpsRemaining > 0) ExecuteJump();
        }
    }

    private void FixedUpdate()
    {
        // --- LA MAGIA DEL DEAD ZONE ---
        float rawInput = InputManager.Instance.MovementX;
        float moveInput = 0f;

        // Solo aceptamos el input si la inclinación es mayor a la zona muerta
        if (Mathf.Abs(rawInput) > deadZone)
        {
            moveInput = rawInput;
        }

        // 1. Aplicar movimiento lateral (ahora filtrado)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 2. Animaciones y Rotación Visual
        if (animator != null)
        {
            animator.SetFloat("Movimiento", Mathf.Abs(moveInput));
        }

        if (moveInput < 0) transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        else if (moveInput > 0) transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        // 3. Limitar posición en X
        Vector2 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -screenBoundX, screenBoundX);
        rb.position = clampedPos;

        // 4. Lógica de caída rápida
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void ExecuteJump()
    {
        jumpsRemaining--; 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Previene saltos hacia la estratosfera
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}