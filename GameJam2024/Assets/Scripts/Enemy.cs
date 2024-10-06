using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool bAttacking = false;

    [SerializeField] private float health = 100f;

    [Header("Movement")]
    [SerializeField] private float hSpeed = 8f;
    [SerializeField] private float vSpeed = 6f;
    [SerializeField] private float vPriority = 1.25f;
    [SerializeField] float movementSmooth = 0.005f;
    /* Whether or not enemy can move */
    [SerializeField] private bool bCanPath = true;

    [Header("Combat")]
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float damageFlashTime = 0.35f;
    [SerializeField] private float hAttackRange = 1.25f;
    [SerializeField] private float vAttackRange = 0.25f;
    [SerializeField] private float hAttackOffset = 0f;
    [SerializeField] private float vAttackOffset = 0f;
    [SerializeField] private float verticalPathingOffset = 0.15f;

    [Header("Animation/States")]
    [SerializeField] private Animator animator;
    private int state;


    private Vector3 velocity = Vector3.zero;

    [Header("Misc")]
    /* OPTIONAL: Used for debugging */
    public string enemyName = "No Name";

    private bool bFacingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Coroutine damageFlashCoroutine = null;

    private bool bDead = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (bDead) {
            return;
        }
        UpdateFSM();

    }

    void UpdateFSM()
    {
        if (bAttacking)
        {
            // Continue to do the attack
            if (animator)
            {
                animator.SetTrigger("Attack1");
            }

            return;
        }

        if (InAttackRange())
        {
            rb.velocity = Vector3.zero;
            StartAttack();
        }
        else if (ShouldPathToPlayer())
        {
            // Go Towards Player
            PathToPosition(GameManager.Instance.player.transform.position);
        }

    }

    private void Move(float hMove, float vMove)
    {
        if (hMove < 0 && bFacingRight) Flip();
        else if (hMove > 0 && !bFacingRight) Flip();

        Vector3 targetVelocity = new Vector2(hMove * hSpeed, vMove * vSpeed);

        Vector2 velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref this.velocity, movementSmooth);
        rb.velocity = velocity;
    }

    private void Flip()
    {
        bFacingRight = !bFacingRight;
        transform.Rotate(0, 180, 0);
    }

    public void TakeDamage(float damage)
    {
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

        GameManager.Instance.enemies.Remove(this);
        Destroy(gameObject);
    }

    bool ShouldPathToPlayer() {
        // TODO: Define path to parameters

        return bCanPath && health > 0f;
    }

    void StartAttack()
    {
        // TODO: Attack

        bAttacking = true;
        StartCoroutine(TEMP_AttackNumerator());
    }

    IEnumerator TEMP_AttackNumerator()
    {
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(attackDuration);
        spriteRenderer.color = Color.white;
        bAttacking = false;
    }

    void PathToPosition(Vector3 position)
    {
        Vector2 direction = position - transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        float hActualRange = hAttackRange + hAttackOffset;
        //float vActualRange = vAttackRange;
        if (distance.x > hActualRange || distance.y > verticalPathingOffset)
        {
            if (distance.x < hActualRange) direction.x = 0;
            if (distance.y < verticalPathingOffset) direction.y = 0;
            else direction.y *= vPriority;
            direction.Normalize();

            Move(direction.x, direction.y);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    bool InAttackRange()
    {
        Vector2 playerPosition = GameManager.Instance.player.transform.position;

        Vector3 distance = GameManager.Instance.player.transform.position - transform.position;
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

    void OnStunned()
    {
        bAttacking = false;
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
