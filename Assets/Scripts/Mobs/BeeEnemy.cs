using System.Collections;
using UnityEngine;

// BeeEnemy: простая state-machine для пчелы (patrol -> chase -> attack -> dead)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BeeEnemy : MonoBehaviour
{
    // Inspector fields
    [Header("Patrol")]
    public Transform leftPoint;           // левая точка патруля
    public Transform rightPoint;          // правая точка патруля
    public float patrolSpeed = 1.5f;      // скорость патруля

    [Header("Chase / Attack")]
    public float detectionRadius = 4f;    // радиус, в котором пчела увидит игрока
    public float attackRange = 1f;        // дистанция, при которой пчела начинает атаковать (кулачком)
    public float chaseSpeed = 3.5f;       // скорость преследования
    public float attackCooldown = 1f;     // интервал между атаками
    public int damage = 15;               // урон игроку при атаке
    public float knockbackForce = 3f;

    [Header("Misc")]
    public LayerMask playerLayer;         // слой, на котором находится Player (или используйте Tag)
    public float stunOnHitTime = 0.2f;    // время небольшого "отскока" после попадания (опция)
    public GameObject deathVFXPrefab;     // эффект при смерти (опционально)
    public float despawnDelay = 1.0f;

    // References (кешированные)
    Rigidbody2D rb;
    Animator animator;
    Transform playerTransform;

    // State
    enum State { Patrol, Chase, Attack, Dead }
    State state = State.Patrol;

    // internal
    private Vector3 currentTarget;
    private bool facingRight = true;      // ориентир для SpriteRenderer flip
    private float lastAttackTime = -999f;
    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Начальный целевой маркер — левая точка, если задана, иначе текущая позиция
        if (leftPoint != null) currentTarget = leftPoint.position;
        else currentTarget = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        // 1) Попытка найти игрока в радиусе обнаружения (Physics2D.OverlapCircle)
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit != null)
        {
            playerTransform = hit.transform;
            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distToPlayer <= attackRange)
            {
                // Если в зоне атаки — переход в state Attack
                state = State.Attack;
            }
            else
            {
                // Если игрок виден, но дальше атаки — преследуем
                state = State.Chase;
            }
        }
        else
        {
            // Игрок не найден — патрулируем
            playerTransform = null;
            state = State.Patrol;
        }

        // Обновляем Animator параметры (например, Speed float)
        float animSpeed = (state == State.Patrol) ? patrolSpeed : (state == State.Chase ? chaseSpeed : 0f);
        animator.SetFloat("Speed", Mathf.Abs(animSpeed)); // предполагаем param "Speed"
    }

    void FixedUpdate()
    {
        if (isDead) return;

        switch (state)
        {
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
        }
    }

    // ------------- State implementations ----------------
    void PatrolUpdate()
    {
        if (leftPoint == null || rightPoint == null) return;

        // движение к текущей цели
        Vector2 pos = rb.position;
        Vector2 targetPos = currentTarget;
        Vector2 dir = (targetPos - (Vector2)pos).normalized;

        rb.MovePosition(pos + dir * patrolSpeed * Time.fixedDeltaTime);

        // если достигли цели (по X), переключаем цель
        float distance = Vector2.Distance(pos, targetPos);
        if (distance < 0.05f)
        {
            // меняем цель
            if (currentTarget == leftPoint.position) currentTarget = rightPoint.position;
            else currentTarget = leftPoint.position;
        }

        // поворачиваем визуал
        HandleFacing(dir.x);
    }

    void ChaseUpdate()
    {
        if (playerTransform == null) return;

        Vector2 pos = rb.position;
        Vector2 targetPos = playerTransform.position;
        Vector2 dir = (targetPos - (Vector2)pos);
        float dist = dir.magnitude;
        if (dist > 0.01f) dir = dir.normalized;
        else dir = Vector2.zero;

        rb.MovePosition(pos + dir * chaseSpeed * Time.fixedDeltaTime);

        HandleFacing(dir.x);
    }

    void AttackUpdate()
    {
        if (playerTransform == null)
        {
            state = State.Patrol;
            return;
        }

        // Смотрим на игрока
        Vector2 toPlayer = playerTransform.position - transform.position;
        HandleFacing(toPlayer.x);

        // Атакуем только если кулдаун прошёл
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            DoAttack();
            lastAttackTime = Time.time;
        }
    }

    void DoAttack()
    {
        // Запустить анимацию атаки
        animator.SetTrigger("Attack"); // предполагается триггер "Attack"

        // Урон наносим в методе, привязанном к анимации (Animation Event) или сразу
        // Простая реализация: делаем OverlapCircle в attackRange
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach (var c in hits)
        {
            // попадаем игрока
            var hp = c.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);

                // опционально — дать отбрасывание игроку (направление от пчелы)
                var playerRb = c.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knock = ((Vector2)c.transform.position - (Vector2)transform.position).normalized;
                    playerRb.AddForce(knock * knockbackForce, ForceMode2D.Impulse); // лёгкий толчок
                }
            }
        }
    }

    // ------------- utility ----------------
    void HandleFacing(float horizontal)
    {
        if (Mathf.Abs(horizontal) < 0.01f) return;
        bool shouldFaceRight = horizontal > 0f;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            // Flip sprite: либо использовать SpriteRenderer.flipX либо localScale.x
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.flipX = !facingRight; // если art смотрит вправо по умолчанию
        }
    }

    // ------------- Death and damage handling --------------
    public void ReceiveDamage(int dmg)
    {
        if (isDead) return;
        // проигрываем хит анимацию
        animator.SetTrigger("Hurt");
        // можно уменьшить здоровье если есть Health компонент:
        var hp = GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(dmg);
            if (hp.Current <= 0) Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        state = State.Dead;

        // выключаем коллайдеры, чтобы больше не мешал
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        // анимация смерти
        animator.SetTrigger("Die"); // параметр "Die"

        // эффект смерти
        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        // уничтожаем через задержку, даём проиграться анимации
        Destroy(gameObject, despawnDelay);
    }

    // Визуализация Radius в сцене
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
