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

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (animator != null) animator.SetTrigger("Die");

        yield return new WaitForSeconds(deathAnimationDuration);
    }
}