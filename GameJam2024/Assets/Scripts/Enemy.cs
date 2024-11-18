using System.Collections;
using UnityEngine;

public enum EnemyType {
    Zombie = 0,
    Witch = 1,

    DEFAULT = 2,
}

public class Enemy : MonoBehaviour
{
    public float health = 100f;

    [Header("Movement")]
    [SerializeField] private float hSpeed = 8f;
    [SerializeField] private float vSpeed = 6f;
    public float vPriority = 1.25f;
    public float separateThreshold = 1.0f;

    [Header("Combat")]
    public float attackDamage = 15f;
    public float attackDuration = 0.8f;
    [SerializeField] private float damageFlashTime = 0.35f;
    public float hAttackRange  = 1.25f;
    [SerializeField] private float vAttackRange = 0.25f;
    public float hAttackOffset = 0f;
    [SerializeField] private float vAttackOffset = 0f;
    public float verticalPathingOffset = 0.15f;
    [SerializeField] private int scoreIncreaseAmount = 100;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Animation/States")]
    [SerializeField] private Animator animator;

    [Header("Misc")]
    /* OPTIONAL: Used for debugging */
    public string enemyName = "No Name";

    public bool bFacingRight = true;
    [HideInInspector] public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    private Coroutine damageFlashCoroutine = null;
    private Coroutine stunnedCoroutine = null;

    private bool bDead = false;

    private EnemyState currentState;

    private EnemyPathState enemyPathState;
    private EnemyAttackState enemyAttackState;
    private EnemyDeathState enemyDeathState;

    public EnemyType enemyType;

    // Enraged Stuff
    private int rageMeter = 0;
    [SerializeField] private int rageThreshold = 2;
    public float enragedTime = 10f;
    private float preRagedHealth;
    private float preRagedHSpeed;
    private float preRagedVSpeed;
    private float preRagedAttackDamage;
    private bool bEnraged = false;
    private float enragedTimer = 0f;

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

        enemyDeathState = new EnemyDeathState(this);

        currentState = enemyPathState;
    }

    private void SetState(EnemyState state) {
        currentState.OnExit();
        currentState = state;
        currentState.OnEnter();
    }

    public void ReturnToDefaultState() {
        SetState(enemyPathState);
    }

    void Update()
    {
        if (bDead) {
            return;
        }

        if (bEnraged) {
            enragedTimer += Time.deltaTime;
            if (enragedTimer > enragedTime) {
                UnEnrageEnemy();
            }
        }

        UpdateStates();

    }

    void UpdateStates()
    {
        if (!enemyAttackState.bAttacking && InAttackRange()) {
            rb.velocity = Vector3.zero;

            SetState(enemyAttackState);
        }

        currentState.OnUpdate();

        // TODO:
        // FLEE STATE
        // ATTACK DAMAGE (TAKE TRIGGER FROM ANIMS TO GET TIMINGS?)
        // STUNNED STATE ?
        
    }

    /// <summary>
    /// Called from animation frame
    /// Purely for cosmetic projectiles, Zombie does not use this
    /// </summary>
    public void ShootCosmeticProjectile() {
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, Quaternion.identity);
        projectile.SetDirection(bFacingRight);
    }

    /// <summary>
    /// Called from animation frame
    /// Damages player IF said player is in range at that anim frame
    /// </summary>
    public void TryDamagePlayer() {
        if (InAttackRange()) {
            GameManager.Instance.player.TakeDamage(attackDamage);
        }
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

            SetState(enemyDeathState);

            StartCoroutine(DeathCoroutine());
        }
    }

    IEnumerator DeathCoroutine() {
        for (int i = 0; i < 2; i++) {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < 2; i++) {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
        spriteRenderer.color = Color.clear;


        GameManager.Instance.score += scoreIncreaseAmount;
        GameManager.Instance.enemiesKilled++;
        GameManager.Instance.enemies.Remove(this);

        GameManager.Instance.OnEnemyKilled(this);

        Destroy(gameObject);
    }

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
        this.animator.SetTrigger("Hurt");
        
        yield return new WaitForSeconds(time);

        spriteRenderer.color = Color.white;

        damageFlashCoroutine = null;
    }

    public void IncrementRageMeter() {
        rageMeter++;

        if (rageMeter > rageThreshold) {
            EnrageEnemy();
        }
    }

    private void EnrageEnemy() {
        bEnraged = true;
        
        preRagedHealth = health;
        health *= 1.5f;

        preRagedHSpeed = HSpeed;
        preRagedVSpeed = VSpeed;
        hSpeed *= 1.5f;
        vSpeed *= 1.5f;

        preRagedAttackDamage = attackDamage;
        attackDamage *= 1.25f;

        spriteRenderer.color = new Color(0.3f, 0.008f, 0.66f);
    }

    private void UnEnrageEnemy() {
        if (health > preRagedHealth) health = preRagedHealth;

        hSpeed = preRagedHSpeed;
        vSpeed = preRagedVSpeed;

        attackDamage = preRagedAttackDamage;

        spriteRenderer.color = Color.white;

        enragedTimer = 0f;
        bEnraged = false;
        rageMeter = 0;
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
