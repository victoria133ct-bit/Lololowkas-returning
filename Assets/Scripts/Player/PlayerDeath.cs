using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    public float deathAnimationDuration = 1f;
    public Transform respawnPoint;


    private Health hp;
    private Animator animator;
    private PlayerMovement movement;
    private PlayerInputHandler inputHandler;

    [SerializeField] private PlayerGameOver gameOverUI;

    private void Awake()
    {
        hp = GetComponent<Health>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        hp.OnDied += OnDied;
    }

    private void OnDisable()
    {
        hp.OnDied -= OnDied;
    }
    void OnDied()
    {
        if (gameOverUI != null)
            gameOverUI.ShowGameOver();

        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        if (inputHandler != null) inputHandler.enabled = false;
        if (movement != null) movement.enabled = false;

        // Останавливаем горизонтальное движение, гравитация работает
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Поворачиваем коллайдер из вертикального в горизонтальный — труп лежит
        var capsule = GetComponent<CapsuleCollider2D>();
        if (capsule != null)
        {
            capsule.direction = CapsuleDirection2D.Horizontal;
            // Меняем местами ширину и высоту чтобы капсула стала лежащей
            capsule.size = new Vector2(capsule.size.y, capsule.size.x);
        }

        if (animator != null) animator.SetTrigger("Die");

        yield return new WaitForSeconds(deathAnimationDuration);
    }
}