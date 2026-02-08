using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;        // Ссылка на Animator (перекидываем в инспектор)
    public PlayerMovement movement;  // Ссылка на PlayerMovement (источник данных)
    public PlayerInputHandler input; // Ссылка на Input для чтения намерений игрока
    SpriteRenderer spriteRenderer;

    public bool useFlipX = true;     // true — используем flipX, false — будем менять scale.x

    float lastNonZeroDir = 1f;      // хранит последний ненулевой знак направления для поворота при остановке

    [Header("Animation Settings")]
    public float speedDeadZone = 0.05f;  // мёртвая зона для устранения дребезга

    private void Awake()
    {
        // Кешируем spriteRenderer. Тянем ссылки, если не назначены в инспекторе.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (animator == null) animator = GetComponent<Animator>();
        if (input == null) input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // ВАЖНО: Берём INPUT игрока, а не физическую скорость!
        // Это предотвращает активацию Run при knockback от врагов
        Vector2 moveInput = input.GetMoveInput();
        float horizontalInput = moveInput.x;
        float absSpeed = Mathf.Abs(horizontalInput);

        // Применяем мёртвую зону - если скорость слишком мала, считаем её нулём
        if (absSpeed < speedDeadZone)
            absSpeed = 0f;

        animator.SetFloat("walkSpeed", absSpeed);

        // IsJumping — логично по состоянию: не на земле и есть вертикальная скорость
        bool isJumping = !movement.IsGrounded && Mathf.Abs(movement.Velocity.y) > 0.01f;
        animator.SetBool("IsJumping", isJumping);

        // Логика поворота / флипа (используем input, а не velocity)
        HandleFacing(horizontalInput);
    }

    private void HandleFacing(float horizontal)
    {
        // Если есть движение вправо или влево — запоминаем направление
        if (Mathf.Abs(horizontal) > 0.01f)
            lastNonZeroDir = Mathf.Sign(horizontal);

        if (useFlipX)
        {
            // flipX = true, если последняя ненулевая скорость была влево (sign < 0)
            spriteRenderer.flipX = lastNonZeroDir < 0f;
        }
        else
        {
            // Меняем scale.x на +1 или -1
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (lastNonZeroDir < 0f ? -1f : 1f);
            transform.localScale = scale;
        }
    }
}