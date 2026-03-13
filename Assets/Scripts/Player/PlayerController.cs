using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 18f; 
    [SerializeField] private float fallMultiplier = 3.5f; 

    [Header("Input Filtering")]
    [Tooltip("Valores por debajo de este número se considerarán como 0 (dispositivo quieto).")]
    [SerializeField] private float deadZone = 0.05f; 

    [Header("Double Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Collision Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Control")]
    [SerializeField] private bool movementEnabled = true; // ✅ Desactiva en menú principal

    public Animator animator;

    [Header("Skin Setup")]
    [SerializeField] private Transform skinContainer;

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

        LoadSkin();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        if (animator != null) 
            animator.SetBool("EnSuelo", isGrounded);

        if (isGrounded && rb.linearVelocity.y <= 0f)
            jumpsRemaining = maxJumps;

        // ✅ Bloquea el input si movimiento desactivado
        if (!movementEnabled) return;

        if (InputManager.Instance != null && InputManager.Instance.WasJumpPressed)
            if (jumpsRemaining > 0) ExecuteJump();
    }

    private void FixedUpdate()
    {
        // ✅ Congela al personaje si movimiento desactivado
        if (!movementEnabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float rawInput = InputManager.Instance != null ? InputManager.Instance.MovementX : 0f;
        float moveInput = 0f;

        if (Mathf.Abs(rawInput) > deadZone)
            moveInput = rawInput;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (animator != null)
            animator.SetFloat("Movimiento", Mathf.Abs(moveInput));

        if (moveInput < 0) transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        else if (moveInput > 0) transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        Vector2 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -screenBoundX, screenBoundX);
        rb.position = clampedPos;

        if (rb.linearVelocity.y < 0 && !isGrounded)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void ExecuteJump()
    {
        jumpsRemaining--; 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void LoadSkin()
    {
        if (GameManager.Instance == null) return;
        
        GameObject skinPrefab = GameManager.Instance.GetSelectedSkinPrefab();
        if (skinPrefab != null && skinContainer != null)
        {
            GameObject mySkin = Instantiate(skinPrefab, skinContainer);
            animator = mySkin.GetComponent<Animator>();
        }
    }

    public void TriggerGameOver()
    {
        enabled = false;
        
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetBool("Derrotado", true);
    }
}