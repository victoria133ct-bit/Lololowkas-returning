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

    private void Awake()
    {
        hp = GetComponent<Health>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        hp.OnDied += onDied;
    }

    private void OnDisable()
    {
        hp.OnDied -= onDied;
    }
    void onDied()
    {
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

        Respawn();
    }
    void Respawn()
    {

        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = Vector3.zero;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;

        hp.Heal(hp.maxHp);

        if (inputHandler != null) inputHandler.enabled = true;
        if (movement != null) movement.enabled = true;
    }
}