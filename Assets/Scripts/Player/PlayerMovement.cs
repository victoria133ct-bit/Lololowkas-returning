using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInputHandler input;

    public AudioSource audioSource;
    public AudioClip jumpSound;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.12f;
    public LayerMask groundLayer;
    public bool IsGrounded { get; private set; }

    // Добавляем свойство Velocity
    public Vector2 Velocity => rb.linearVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void FixedUpdate()
    {
        Vector2 move = input.GetMoveInput();
        rb.linearVelocity = new Vector2(move.x * moveSpeed, rb.linearVelocity.y);

        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (input.ConsumeJump() && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            audioSource.PlayOneShot(jumpSound);
        }
    }
}
