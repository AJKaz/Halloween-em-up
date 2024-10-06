using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool bAttacking = false;

    [SerializeField] private float health = 100f;

    [Header("Movement")]
    [SerializeField] private float hSpeed = 8f;
    [SerializeField] private float vSpeed = 6f;
    [SerializeField] private float vPriority = 1.25f;
    [SerializeField] private float hAttackRange = 1.25f;
    [SerializeField] private float vAttackRange = 0.25f;
    [SerializeField] float movementSmooth = 0.005f;
    /* Whether or not enemy can move */
    [SerializeField] private bool bCanPath = true;

    [Header("Combat")]
    [SerializeField] private float attackDuration = 0.35f;
    [SerializeField] private float damageFlashTime = 0.35f;


    private Vector3 velocity = Vector3.zero;

    [Header("Misc")]
    /* OPTIONAL: Used for debugging */
    public string enemyName = "No Name";

    private bool bFacingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Coroutine damageFlashCoroutine = null;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateFSM();
    }

    void UpdateFSM() {
        if (bAttacking) {
            // Continue to do the attack
            
            return;
        }

        if (InAttackRange()) {
            rb.velocity = Vector3.zero;
            StartAttack();
        }
        else if (ShouldPathToPlayer()) {
            // Go Towards Player
            PathToPosition(GameManager.Instance.player.transform.position);
        }

    }

    private void Move(float hMove, float vMove) {
        if (hMove < 0 && bFacingRight) Flip();
        else if (hMove > 0 && !bFacingRight) Flip();

        Vector3 targetVelocity = new Vector2(hMove * hSpeed, vMove * vSpeed);

        Vector2 velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref this.velocity, movementSmooth);
        rb.velocity = velocity;
    }

    private void Flip() {
        bFacingRight = !bFacingRight;
        transform.Rotate(0, 180, 0);
    }

    public void TakeDamage(float damage) {
        if (damageFlashCoroutine != null) {
            StopCoroutine(damageFlashCoroutine);
        }
        damageFlashCoroutine = StartCoroutine(FlashTint(damageFlashTime, Color.red));
        
        health -= damage;
        if (health <= 0) {
            spriteRenderer.color = Color.black;
        }
    }

    bool ShouldPathToPlayer() {
        // TODO: Define path to parameters

        return bCanPath && health > 0f;
    }

    void StartAttack() {
        // TODO: Attack

        //StartCoroutine(FlashTint(0.25f, Color.blue));
    }

    void PathToPosition(Vector3 position) {
        Vector2 direction = position - transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        if (distance.x > hAttackRange || distance.y > vAttackRange) {
            if (distance.x < hAttackRange) direction.x = 0;
            if (distance.y < vAttackRange) direction.y = 0;
            else direction.y *= vPriority;
            direction.Normalize();

            Move(direction.x, direction.y);
        }
        else {
            rb.velocity = Vector3.zero;
        }
    }

    bool InAttackRange() {
        Vector3 distance = GameManager.Instance.player.transform.position - transform.position;
        return Mathf.Abs(distance.x) <= hAttackRange && Mathf.Abs(distance.y) <= vAttackRange;
    }

    void OnStunned() {
        bAttacking = false;
    }

    private IEnumerator FlashTint(float time, Color color) {
        spriteRenderer.color = color;

        yield return new WaitForSeconds(time);

        spriteRenderer.color = Color.white;

        damageFlashCoroutine = null;
    }
}
