using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    public Transform respawnPoint;

    private Health hp;

    private void Awake()
    {
        hp = GetComponent<Health>();
    }

    private void OnEnable()
    {
        hp.OnDied += Respawn;
    }

    private void OnDisable()
    {
        hp.OnDied -= Respawn;
    }

    void Respawn()
    {

        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = Vector3.zero;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        hp.Heal(hp.maxHp);
    }
}