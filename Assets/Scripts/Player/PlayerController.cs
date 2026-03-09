using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 18f; 
    
    [Header("Jump Tweaks (Snappy Jump)")]
    [SerializeField] private float fallMultiplier = 3.5f; 
    
    // NUEVO: Cooldown para evitar que la física acumule multiplicadores de fuerza indeseados
    [SerializeField] private float jumpCooldown = 0.1f; 

    [Header("Collision Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Animator animator;
    
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private float screenBoundX;
    
    private float lastJumpTime; // Memoria del último salto

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
        float playerHalfWidth = col.bounds.extents.x;
        screenBoundX = (Camera.main.orthographicSize * Camera.main.aspect) - playerHalfWidth;
        
        // ELIMINADO: La suscripción al evento OnJumpPressed ya no es necesaria
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("EnSuelo", isGrounded);
    }

    private void FixedUpdate()
    {
        // 1. Aplicar movimiento lateral
        float moveInput = InputManager.Instance.MovementX;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        float velocidadAbsoluta = Mathf.Abs(moveInput);
        animator.SetFloat("Movimiento", velocidadAbsoluta);

        // Girar el personaje según dirección
        if (moveInput < 0)
        {
            transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }

        // 2. Limitar posición en X
        Vector2 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -screenBoundX, screenBoundX);
        rb.position = clampedPos;

        // 3. LÓGICA DE AUTO-SALTO CONTINUO
        // Si la pantalla está pulsada, está tocando el suelo, y ya pasó el tiempo de enfriamiento
        if (InputManager.Instance.IsJumpHeld && isGrounded)
        {
            Jump();
        }

        // 4. Lógica de caída rápida
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        // Validación de seguridad para la inercia física
        if (Time.time >= lastJumpTime + jumpCooldown)
        {
            lastJumpTime = Time.time;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}