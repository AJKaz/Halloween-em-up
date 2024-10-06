using System.Buffers.Text;
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
    [SerializeField] private float runAwaySpeedMultiplier = 1.5f;
    [SerializeField] private float hAttackRange = 1.25f;
    [SerializeField] private float vAttackRange = 0.25f;


    private Vector3 velocity = Vector3.zero;
    [SerializeField] float movementSmooth = 0.005f;

    public string enemyName = "No Name";

    private Rigidbody2D rb;
    private bool facingRight = true;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
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
        else if (ShouldRun()) {
            RunAway();
        }

    }

    private void Move(float hMove, float vMove) {
        if (hMove < 0 && facingRight) Flip();
        else if (hMove > 0 && !facingRight) Flip();

        Vector3 targetVelocity = new Vector2(hMove * hSpeed, vMove * vSpeed);

        Vector2 velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref this.velocity, movementSmooth);
        rb.velocity = velocity;
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    bool ShouldPathToPlayer() {
        // TODO: Define path to parameters

        return health > 0f;
    }

    bool ShouldRun() {
        return health <= 0f;
    }

    void StartAttack() {
        // TODO: Attack
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

    void RunAway() {
        // TODO: Make direciton dynamic
        // IF on left half of screen, run left
        // IF on right half of screen, run right
        // Can have some verticality to this to make it unique, but make it slight
        // Note: It's okay if this is run every frame, technically worse for performance but ok for this scale
        Vector2 direction = new Vector2(-1, 0);
        direction.Normalize();
       //Move(direction.x, direction.y, false);
    }

    bool InAttackRange() {
        Vector3 distance = GameManager.Instance.player.transform.position - transform.position;
        return Mathf.Abs(distance.x) <= hAttackRange && Mathf.Abs(distance.y) <= vAttackRange;
    }

    void OnStunned() {
        bAttacking = false;
    }
}
