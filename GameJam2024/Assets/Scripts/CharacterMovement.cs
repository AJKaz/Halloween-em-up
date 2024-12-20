using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private bool canMove = true;
    
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
    private bool bTouchingLeftBounds = false;
    private bool bTouchingRightBounds = false;

    private Vector3 velocity = Vector3.zero;

    PlayerInput input;
    Controls controls = new Controls();

    [Header("Combat State")]
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float hAttackRange = 1.5f;
    [SerializeField] private float vAttackRange = 0.5f;
    [SerializeField] private float vOffset = 0.5f;
    [SerializeField] private float hOffset = 0.5f;
    private StateMachine meleeStateMachine;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine damageCoroutine;
    private float flashTime = .35f;

    private AudioClip attack1;
    private AudioClip attack2;
    private AudioClip attack3;
    private AudioClip hurt;
    [SerializeField] private AudioSource soundManager;

    public float health = 100f;
    
    private bool invul = false;

    private float timeAlive = 0f;

    public bool grounded { get { return onBase; } }

    public float TimeAlive { get { return timeAlive; } }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        vSpeed = groundVSpeed;

        meleeStateMachine = GetComponent<StateMachine>();
    }

    private void Start()
    {
        attack1 = (AudioClip)Resources.Load("attack1");
        attack2 = (AudioClip)Resources.Load("attack2");
        attack3 = (AudioClip)Resources.Load("attack3");
        hurt = (AudioClip)Resources.Load("hurt");

        timeAlive = 0f;
    }

    private void Update()
    {
        if (GameMenus.bGamePaused) return;

        timeAlive += Time.deltaTime;

        controls = input.GetInput();
        if (controls.JumpState && currentJumps < possibleJumps)
        {
            jump = true;
        }

        if (controls.AttackState)
        {
            Attack();
        }

        //Animation Controller jazz
        GameManager.Instance.playerAnimator.SetBool("Walking", (baseRB.velocity.magnitude >= 0.4));
        GameManager.Instance.playerAnimator.SetBool("InAir", !onBase);
        if (!onBase)
        {
            GameManager.Instance.playerAnimator.SetBool("Walking", false);
        }
    }

    private void FixedUpdate()
    {
        if (GameMenus.bGamePaused) return;

        Move();
    }
    
    public void TakeDamage(float damage)
    {
        if (invul == true)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            GameMenus.Instance.EnableLoseScreen();
        }
        
        damageCoroutine = StartCoroutine(FlashTint(flashTime, Color.red));
    }

    private IEnumerator FlashTint(float time, Color color)
    {
        //Hit stop
        soundManager.clip = hurt;
        soundManager.Play();
        GameManager.Instance.playerAnimator.SetBool("Hurt",true);
        FindObjectOfType<HitStop>().TimeStop(0.1f);
        invul = true;
        spriteRenderer.color = color;



        yield return new WaitForSeconds(time);
        GameManager.Instance.playerAnimator.SetBool("Hurt", false);
        soundManager.clip = attack1;
        damageCoroutine = StartCoroutine(Invul(1f));
    }

    private IEnumerator Invul(float time)
    {
        spriteRenderer.color = new Color(1, 1, 1, .5f);

        yield return new WaitForSeconds(time);
        spriteRenderer.color = Color.white;
        damageCoroutine = null;
        invul = false;
    }

    private void Move()
    {
        if (!onBase && doesCharacterJump)
        {
            DetectBase();
        }

        if (canMove)
        {

            if (bTouchingUpperBounds && controls.VerticalMove > 0) controls.VerticalMove = 0;
            if (bTouchingLowerBounds && controls.VerticalMove < 0) controls.VerticalMove = 0;
            if (bTouchingLeftBounds && controls.HorizontalMove < 0) controls.HorizontalMove = 0;
            if (bTouchingRightBounds && controls.HorizontalMove > 0) controls.HorizontalMove = 0;

            if (jump)
            {
                charRB.velocity = new Vector2(charRB.velocity.x, 0);
                charRB.AddForce(Vector2.up * jumpVal, ForceMode2D.Impulse);
                //charRB.velocity = new Vector2(charRB.velocity.x, Mathf.Clamp(charRB.velocity.y, -5, 8));
                GameManager.Instance.playerAnimator.SetTrigger("Rising");
                charRB.gravityScale = jumpingGravityScale;
                jump = false;
                currentJumps++;
                onBase = false;

                vSpeed = airVSpeed;
            }



            Vector3 targetVelocity = new Vector2(controls.HorizontalMove * hSpeed, controls.VerticalMove * vSpeed);

            Vector2 velocity = Vector3.SmoothDamp(baseRB.velocity, targetVelocity, ref this.velocity, movementSmooth);
            baseRB.velocity = velocity;

            if (doesCharacterJump)
            {
                if (onBase)
                {
                    // on base
                    charRB.velocity = velocity;
                    vSpeed = groundVSpeed;


                }
                else
                {
                    // in air
                    if (charRB.velocity.y < 0)
                    {
                        charRB.gravityScale = fallingGravityScale;
                    }
                    charRB.velocity = new Vector2(velocity.x, charRB.velocity.y);
                }
            }

            // rotate if we're facing the wrong way
            if (controls.HorizontalMove > 0 && !facingRight)
            {
                Flip();
            }
            else if (controls.HorizontalMove < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    public void Attack()
    {

        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new MeleeEntryState());
        }
    }

    //From sprite script managing when attacks are triggered from part of animation and how much hit stop based on the attack
    public void DamageEnemyTrigger(float time)
    {
        List<Enemy> enemiesToHit = GetEnemiesInRange();
        foreach (Enemy enemy in enemiesToHit)
        {
            enemy.TakeDamage(attackDamage);
            //Hit stop
            FindObjectOfType<HitStop>().TimeStop(time);
            soundManager.Play();
        }
        
    }

    public void SoundTrigger(int attack)
    {
        AudioClip attackSound = attack1;
        switch (attack)
        {
            case 0:
                attackSound = attack1;
                break;
            case 1: 
                attackSound = attack2;
                break;
            case 2:
                attackSound = attack3;
                break;
        }
        soundManager.clip = attackSound;
    }

    public List<Enemy> GetEnemiesInRange()
    {
        // WARNING: ASSUMES PLAYER AND ENEMIES ARE GROUNDED

        List<Enemy> enemiesInRange = new List<Enemy>();

        Vector2 playerPosition = transform.position;

        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            Vector2 enemyPosition = enemy.transform.position;

            float vDistance = Mathf.Abs(enemyPosition.y - (playerPosition.y + vOffset));
            if (vDistance <= vAttackRange)
            {
                // Within vertical attack range, now check horizontal attack range
                float hDistance = enemyPosition.x - playerPosition.x - (facingRight ? hOffset : -hOffset);

                if (facingRight && hDistance >= 0 && hDistance <= hAttackRange)
                {
                    enemiesInRange.Add(enemy);
                }
                else if (!facingRight && hDistance <= 0 && Mathf.Abs(hDistance) <= hAttackRange)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        return enemiesInRange;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void DetectBase()
    {
        if (!onBase && charRB.velocity.y <= 0)
        {
            RaycastHit2D fall = Physics2D.Raycast(jumpDetector.position, -Vector2.up, .05f, detectLayer);
            if (fall.collider != null)
            {
                GameManager.Instance.playerAnimator.SetTrigger("Falling");
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(jumpDetector.position, -Vector2.up, detectionDistance, detectLayer);
        if (hit.collider != null)
        {

            onBase = true;
            currentJumps = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UpperBounds"))
        {
            bTouchingUpperBounds = true;
        }
        else if (collision.CompareTag("LowerBounds"))
        {
            bTouchingLowerBounds = true;
        }

        if (collision.CompareTag("LeftBounds"))
        {
            bTouchingLeftBounds = true;
        }
        else if (collision.CompareTag("RightBounds"))
        {
            bTouchingRightBounds = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("UpperBounds"))
        {
            bTouchingUpperBounds = false;
        }
        else if (collision.CompareTag("LowerBounds"))
        {
            bTouchingLowerBounds = false;
        }

        if (collision.CompareTag("LeftBounds"))
        {
            bTouchingLeftBounds = false;
        }
        else if (collision.CompareTag("RightBounds"))
        {
            bTouchingRightBounds = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (doesCharacterJump)
        {
            Gizmos.DrawRay(jumpDetector.transform.position, -Vector3.up * detectionDistance);
            Gizmos.DrawRay(jumpDetector.transform.position, -Vector3.up * .25f);
        }

        DrawAttackRange();

    }

    private void DrawAttackRange()
    {
        Gizmos.color = Color.cyan;

        Vector2 playerPos = transform.position;

        Vector2 boxCenter = playerPos + new Vector2(facingRight ? (hAttackRange / 2) + hOffset : -(hAttackRange / 2) - hOffset, vOffset);
        Gizmos.DrawWireCube(boxCenter, new Vector2(hAttackRange, vAttackRange * 2));
    }

    private void DrawBoxCollider(BoxCollider2D collider, Color color)
    {
        Gizmos.color = color;

        Vector2 size = collider.size;
        Vector2 offset = collider.offset;

        Vector2 position = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(position, size);
    }
}
