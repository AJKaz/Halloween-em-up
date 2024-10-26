using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    [Header("Movement")]
    [SerializeField] private float hSpeed = 8f;
    [SerializeField] private float vSpeed = 6f;
    public float vPriority = 1.25f;
    public float separateThreshold = 1.0f;
    /* Whether or not enemy can move */
    [SerializeField] private bool bCanPath = true;

    [Header("Combat")]
    [SerializeField] private float attackDamage = 15f;
    public float attackDuration = 0.8f;
    [SerializeField] private float damageFlashTime = 0.35f;
    public float hAttackRange  = 1.25f;
    [SerializeField] private float vAttackRange = 0.25f;
    public float hAttackOffset = 0f;
    [SerializeField] private float vAttackOffset = 0f;
    public float verticalPathingOffset = 0.15f;

    [Header("Animation/States")]
    [SerializeField] private Animator animator;

    [Header("Misc")]
    /* OPTIONAL: Used for debugging */
    public string enemyName = "No Name";

    public bool bFacingRight = true;
    [HideInInspector] public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Coroutine damageFlashCoroutine = null;
    private Coroutine stunnedCoroutine = null;

    private bool bDead = false;

    private EnemyState currentState;

    private EnemyPathState enemyPathState;
    private EnemyAttackState enemyAttackState;

    public float HSpeed { get { return hSpeed; } }
    public float VSpeed { get { return vSpeed; } }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        enemyPathState = new EnemyPathState(this);
        
        enemyAttackState = new EnemyAttackState(this);
        enemyAttackState.SetAnimator(animator);

        currentState = enemyPathState;
    }

    public void ReturnToDefaultState() {
        currentState.OnExit();
        currentState = enemyPathState;
        currentState.OnEnter();
    }

    void Update()
    {
        if (bDead) {
            return;
        }
        UpdateStates();

    }

    void UpdateStates()
    {
        if (!enemyAttackState.bAttacking && InAttackRange()) {
            rb.velocity = Vector3.zero;
            currentState.OnExit();
            currentState = enemyAttackState;
            currentState.OnEnter();
        }

        currentState.OnUpdate();

        // TODO:
        // FLEE STATE
        // ATTACK DAMAGE (TAKE TRIGGER FROM ANIMS TO GET TIMINGS?)
        // STUNNED STATE ?
        
    }

    public void TakeDamage(float damage)
    {
        if (stunnedCoroutine != null) {
            StopCoroutine(stunnedCoroutine);
        }
        stunnedCoroutine = StartCoroutine(StunCoroutine());

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }
        damageFlashCoroutine = StartCoroutine(FlashTint(damageFlashTime, Color.red));

        health -= damage;
        if (health <= 0) {
            bDead = true;
            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine() {
        for (int i = 0; i < 3; i++) {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.45f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.45f);
        }
        for (int i = 0; i < 3; i++) {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
       
        spriteRenderer.color = Color.clear;
        GameManager.Instance.score++;
        GameManager.Instance.enemiesKilled++;
        GameManager.Instance.enemies.Remove(this);
        Destroy(gameObject);
    }

    bool ShouldPathToPlayer() {
        // TODO: Define path to parameters

        return bCanPath && health > 0f;
    }


    /*void StartAttack()
    {
        if (animator) {
            animator.SetTrigger("Attack1");
        }
        StartCoroutine(AttackCoroutine());
    }*/

    /*IEnumerator AttackCoroutine()
    {
        bAttacking = true;

        yield return new WaitForSeconds(0.15f);

        GameManager.Instance.player.TakeDamage(attackDamage);

        yield return new WaitForSeconds(attackDuration - 0.15f);
        bAttacking = false;
    }*/

    public void Flip() {
        bFacingRight = !bFacingRight;
        transform.Rotate(0, 180, 0);
    }

    bool InAttackRange()
    {
        Vector3 playerPosition = GameManager.Instance.player.transform.position;

        if (bFacingRight && playerPosition.x < transform.position.x || !bFacingRight && playerPosition.x > transform.position.x) {
            Flip();
        }

        Vector3 distance = playerPosition - transform.position;
        distance.x = Mathf.Abs(distance.x);
        distance.y = Mathf.Abs(distance.y);

        float vDistance = Mathf.Abs(playerPosition.y - (transform.position.y + vAttackOffset));
        if (vDistance <= vAttackRange)
        {
            // Within vertical attack range, now check horizontal attack range
            float hDistance = playerPosition.x - transform.position.x - (bFacingRight ? hAttackOffset : -hAttackOffset);

            if (bFacingRight && hDistance >= 0 && hDistance <= hAttackRange)
            {
                return true;
            }
            if (!bFacingRight && hDistance <= 0 && Mathf.Abs(hDistance) <= hAttackRange)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator StunCoroutine() {
        //bStunned = true;

        yield return new WaitForSeconds(damageFlashTime);

       // bStunned = false;
        stunnedCoroutine = null;
    }

    private IEnumerator FlashTint(float time, Color color)
    {
        spriteRenderer.color = color;
        
        yield return new WaitForSeconds(time);

        spriteRenderer.color = Color.white;

        damageFlashCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        DrawAttackRange();

    }
    private void DrawAttackRange()
    {
        Gizmos.color = Color.gray;

        Vector2 enemyPos = transform.position;
        Vector2 boxCenter = enemyPos + new Vector2(bFacingRight ? (hAttackRange / 2) + hAttackOffset : -(hAttackRange / 2) - hAttackOffset, vAttackOffset);
        Gizmos.DrawWireCube(boxCenter, new Vector2(hAttackRange, vAttackRange * 2));
    }
}
