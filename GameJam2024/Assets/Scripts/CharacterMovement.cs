using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    [SerializeField] private bool canMove = true;
    [Tooltip(("If your character does not jump, ignore all below 'Jumping Character'"))]
    [SerializeField] private bool doesCharacterJump = false;

    [Header("Base / Root")]
    [SerializeField] private Rigidbody2D baseRB;
    [SerializeField] private float hSpeed = 10f;
    private float vSpeed;
    [SerializeField] private float groundVSpeed = 6f;
    [SerializeField] private float airVSpeed = 1f;
    [Range(0, 1.0f)]
    [SerializeField] float movementSmooth = 0.5f;

    [Header("'Jumping' Character")]
    [SerializeField] private Rigidbody2D charRB;
    [SerializeField] private float jumpVal = 10f;
    [SerializeField] private int possibleJumps = 1;
    [SerializeField] private int currentJumps = 0;
    [SerializeField] private bool onBase = false;
    [SerializeField] private Transform jumpDetector;
    [SerializeField] private float detectionDistance;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private float jumpingGravityScale;
    [SerializeField] private float fallingGravityScale;
    private bool jump;

    private bool facingRight = true;

    private bool bTouchingUpperBounds = false;
    private bool bTouchingLowerBounds = false;

    private Vector3 velocity = Vector3.zero;

    PlayerInput input;
    Controls controls = new Controls();

    [Header("Combat State")]
    [SerializeField] private float hAttackRange = 1.5f;
    [SerializeField] private float vAttackRange = 0.5f;
    [SerializeField] private float vOffset = 0.5f;
    [SerializeField] private float hOffset = 0.5f;
    private StateMachine meleeStateMachine;

    public bool grounded { get { return onBase; } }


    private void Awake() {
        input = GetComponent<PlayerInput>();
        vSpeed = groundVSpeed;

        meleeStateMachine = GetComponent<StateMachine>();
    }

    private void Update() {
        controls = input.GetInput();
        if (controls.JumpState && currentJumps < possibleJumps) {
            jump = true;
        }

        if (controls.AttackState) {
            Attack();
        }
    }

    private void FixedUpdate() {
        Move();
    }

    private void Move() {
        if (!onBase && doesCharacterJump) {
            DetectBase();
        }

        if (canMove) {

            if (bTouchingUpperBounds && controls.VerticalMove > 0) controls.VerticalMove = 0;
            if (bTouchingLowerBounds && controls.VerticalMove < 0) controls.VerticalMove = 0;

            if (jump) {
                charRB.velocity = new Vector2(charRB.velocity.x, 0);
                charRB.AddForce(Vector2.up * jumpVal, ForceMode2D.Impulse);
                //charRB.velocity = new Vector2(charRB.velocity.x, Mathf.Clamp(charRB.velocity.y, -5, 8));
                
                charRB.gravityScale = jumpingGravityScale;
                jump = false;
                currentJumps++;
                onBase = false;
                
                vSpeed = airVSpeed;
            }

            

            Vector3 targetVelocity = new Vector2(controls.HorizontalMove * hSpeed, controls.VerticalMove * vSpeed);

            Vector2 velocity = Vector3.SmoothDamp(baseRB.velocity, targetVelocity, ref this.velocity, movementSmooth);
            baseRB.velocity = velocity;

            if (baseRB.velocity.x > 0 && baseRB.velocity.x < 0)
            {
                //walking
                GameManager.Instance.playerAnimator.SetTrigger("walking");
            }

            if (doesCharacterJump) {
                if (onBase) {
                    // on base
                    charRB.velocity = velocity;
                    vSpeed = groundVSpeed;
                }
                else {
                    // in air
                    if (charRB.velocity.y < 0) {
                        charRB.gravityScale = fallingGravityScale;
                    }

                    charRB.velocity = new Vector2(velocity.x, charRB.velocity.y);
                }
            }

            // rotate if we're facing the wrong way
            if (controls.HorizontalMove > 0 && !facingRight) {
                Flip();
            }
            else if (controls.HorizontalMove < 0 && facingRight) {
                Flip();
            }
        }

       
    }

    public void Attack() {
        Debug.Log("Attack");

        List<Enemy> enemiesToHit = GetEnemiesInRange();
        foreach (Enemy enemy in enemiesToHit) {
            Debug.Log("Hit Enemy " + enemy.enemyName);
        }

        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            Debug.Log("Attack");
            meleeStateMachine.SetNextState(new MeleeEntryState());
        }
    

    }

    public List<Enemy> GetEnemiesInRange() {
        // WARNING: ASSUMES PLAYER AND ENEMIES ARE GROUNDED

        List<Enemy> enemiesInRange = new List<Enemy>();

        Vector2 playerPosition = transform.position;
        
        foreach (Enemy enemy in GameManager.Instance.enemies) {
            Vector2 enemyPosition = enemy.transform.position;

            float vDistance = Mathf.Abs(enemyPosition.y - (playerPosition.y + vOffset));
            if (vDistance <= vAttackRange) {
                // Within vertical attack range, now check horizontal attack range
                float hDistance = enemyPosition.x - playerPosition.x - (facingRight ? hOffset : -hOffset);
                
                if (facingRight && hDistance >= 0 && hDistance <= hAttackRange) {
                    enemiesInRange.Add(enemy);
                }
                else if (!facingRight && hDistance <= 0 && Mathf.Abs(hDistance)  <= hAttackRange) {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        return enemiesInRange;
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void DetectBase() {
        RaycastHit2D hit = Physics2D.Raycast(jumpDetector.position, -Vector2.up, detectionDistance, detectLayer);
        if (hit.collider != null) {
            onBase = true;
            currentJumps = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("UpperBounds")) {
            bTouchingUpperBounds = true;
        }
        else if (collision.CompareTag("LowerBounds")) {
            bTouchingLowerBounds = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("UpperBounds")) {
            bTouchingUpperBounds = false;
        }
        else if (collision.CompareTag("LowerBounds")) {
            bTouchingLowerBounds = false;
        }
    }

    private void OnDrawGizmos() {
        if (doesCharacterJump) {
            Gizmos.DrawRay(jumpDetector.transform.position, -Vector3.up * detectionDistance);
        }

        DrawAttackRange();

    }

    private void DrawAttackRange() {
        Gizmos.color = Color.cyan;

        Vector2 playerPos = transform.position;

        Vector2 boxCenter = playerPos + new Vector2(facingRight ? (hAttackRange / 2) + hOffset : -(hAttackRange / 2) - hOffset, vOffset);
        Gizmos.DrawWireCube(boxCenter, new Vector2(hAttackRange, vAttackRange * 2));
    }

    private void DrawBoxCollider(BoxCollider2D collider, Color color) {
        Gizmos.color = color;

        Vector2 size = collider.size;
        Vector2 offset = collider.offset;

        Vector2 position = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(position, size);
    }
}
