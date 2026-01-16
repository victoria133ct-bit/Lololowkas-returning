using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;       // пустышка перед игроком
    public float attackRange = 0.8f;
    public int damage = 25;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    float lastAttackTime = -999f;

    [Header("References")]
    public PlayerInputHandler inputHandler; // drag the PlayerInputHandler here (or GetComponent)
    public Animator animator;               // animator (можно auto-get)

    private void Awake()
    {
        if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
        if (animator == null) animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (inputHandler != null && inputHandler.ConsumeAttack())
        {
            // Запустить анимацию атаки. Урон будет нанесён через Animation Event (OnAttackHit)
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    public void OnAttackHit()
    {
        // Наносим урон всем врагам в радиусе
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint not set on PlayerAttack.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var c in hits)
        {
            var hp = c.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            else
            {

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
