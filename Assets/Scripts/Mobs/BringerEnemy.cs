using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BringerEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 6f;
    public float meleeRange = 1.2f;
    public float spellRange = 4f;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float walkSpeed = 1.8f;

    [Header("Combat")]
    public int meleeDamage = 20;
    public int spellDamage = 15;
    public float attackCooldown = 1.5f;
    public GameObject spellPrefab;

    Rigidbody2D rb;
    Animator animator;
    Transform playerTransform;
    SpriteRenderer sr;

    enum State { Idle, Chase, MeleeAttack, CastSpell, Dead }
    State state = State.Idle;

    bool facingRight = true;
    float lastAttackTime = -999f;
    bool isAttacking = false;
    bool isDead = false;

    void Awake()
    {
        // TODO: ???????? ?????? rb ? animator ????? GetComponent
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // TODO: ??????????? ?? Health.OnDied (??? ? BeeEnemy)
        var hp = GetComponent<Health>();
        if (hp != null)
            hp.OnDied += Die;
    }

    void OnDisable()
    {
        // TODO: ??????????
        var hp = GetComponent<Health>();
        if (hp != null)
            hp.OnDied -= Die;
    }

    void Update()
    {
        if (isDead) return;

        // TODO 1: Physics2D.OverlapCircle — ????? ??????
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        // TODO 2: ???? ?????? — ?????????? IsDead (?????????? ????)
        // TODO 3: ???? ????? ?? ?????? — state = Idle, ????????????, return
        if(hit != null)
        {
            var playerHp = hit.GetComponent<Health>();

            if (playerHp != null && playerHp.IsDead)
                hit = null;
        }
        if(hit == null)
        {
            state = State.Idle;
            rb.linearVelocity = Vector3.zero;
            return;
        }
        playerTransform = hit.transform;

        // TODO 4: ????????? ????????? ?? ??????
        Vector2 targetPos = (Vector2)playerTransform.position;
        float distToTarget = Vector2.Distance(transform.position, targetPos);

        // ????? ????????? ?? ?????????:
        if (!isAttacking)
        {
            // TODO 5: ???? dist <= meleeRange ? state = MeleeAttack
            if(distToTarget <= meleeRange)
            {
                state = State.MeleeAttack;
            }
            // TODO 6: ????? ???? dist <= spellRange ? state = CastSpell
            else if (distToTarget <= spellRange)
            {
                state = State.CastSpell;
            }
            // TODO 7: ????? ? state = Chase
            else
            {
                state = State.Chase;
            }
        }

        // ???????? ?? ?????????:
        switch (state)
        {
            case State.Chase:
                Chase();
                break;
            case State.MeleeAttack:
                TryMeleeAttack();
                break;
            case State.CastSpell:
                TryCastSpell();
                break;
        }
    }

    void Chase()
    {
        Vector2 pos = transform.position;
        Vector2 targetPos = playerTransform.position;

        // TODO: ????????? ? ?????? ?? X ????? rb.linearVelocity
        Vector2 dir = (targetPos - pos).normalized;
        rb.linearVelocity = new Vector2(dir.x * walkSpeed, rb.linearVelocity.y);
        // TODO: animator.SetBool("IsWalking", true)
        animator.SetBool("IsWalking", true);
        // TODO: HandleFacing(???????????)
        HandleFacing(dir.x);
    }

    void TryMeleeAttack()
    {
        rb.linearVelocity = Vector2.zero;
        // TODO: ????????????, ??????????? ? ??????
        animator.SetBool("IsWalking", false);

        HandleFacing(playerTransform.position.x - transform.position.x);
        // TODO: ????????? ??????? — Time.time - lastAttackTime
        // TODO: ???? ??????? ?????? — animator.SetTrigger("Attack"), isAttacking = true
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    void TryCastSpell()
    {
        // TODO: ?? ?? ??? MeleeAttack, ?? animator.SetTrigger("Cast")
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsWalking", false);

        HandleFacing(playerTransform.position.x - transform.position.x);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Cast");
            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    // ===== Animation Events =====

    public void OnAttackHit()
    {
        // TODO: OverlapCircle ????? ????????
        Collider2D hit = Physics2D.OverlapCircle(transform.position, meleeRange, playerLayer);
        // TODO: ????? Health ? ?????? ? TakeDamage(meleeDamage)

        if (hit != null)
        {
            var hp = hit.GetComponent<Health>();
            if (hp != null)
                hp.TakeDamage(meleeDamage);
        }
    }

    public void OnSpellSpawn()
    {
        // TODO: ????????? ??? spellPrefab ? playerTransform ?? null
        if (spellPrefab == null || playerTransform == null) return;
        // TODO: Instantiate ?????? ? ??????? ??????
        Instantiate(spellPrefab, playerTransform.position, Quaternion.identity);
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
    }

    // ===== Utility =====

    void HandleFacing(float horizontal)
    {
        // TODO: ????????? ?????? ????? sr.flipX
        if (horizontal > 0)
            sr.flipX = false;
        else if (horizontal < 0)
            sr.flipX = true;
    }

    void Die()
    {
        // TODO: isDead = true, ????????? ??????????, SetTrigger("Die"), Destroy ????? delay
        if (isDead) return;
        isDead = true;
        state = State.Dead;

        // ????????? ??????????, ????? ?????? ?? ?????
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // ?????????? ????? (???? ??? ???????? ??????)
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = new Color(0.7f, 0.3f, 1f);
        Gizmos.DrawWireSphere(transform.position, spellRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
