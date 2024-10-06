using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float health = 500f;
    

    [Header("Ground")]
    [SerializeField] protected bool canMove = true;
    [SerializeField] private float hSpeed = 5000f;
    [SerializeField] private float vSpeed = 3500f;
    [Range(0, 1.0f)]
    [SerializeField] protected Rigidbody2D trueRB;
    [SerializeField] private float clampValue = 5;
    private bool faceRight = true;

    [Header("Jump")]
    [SerializeField] private Rigidbody2D spriteRB;
    [SerializeField] float jumpForce = 7f;
    public bool grounded = true;
    [SerializeField] private float detectionDistance = 1.05f;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private float risingGravity = 1;
    [SerializeField] private float fallingGravity = 1.5f;

    [Header("Combat State")]
    private StateMachine meleeStateMachine;
    [SerializeField] private Collider hurtbox;
    [SerializeField] private Collider hitbox;
    public void Awake()
    {
        meleeStateMachine = GetComponent<StateMachine>();

    }

    public void Move(float hMove, float vMove, bool jump)
    {
        if (canMove)
        {

            //Debug.Log("transform " + spriteRB.transform.position);
            //Debug.Log("Velocity " + spriteRB.velocity);
            //Debug.Log("Vmove" + vMove);

            if (!grounded)
            {

                if (spriteRB.velocity.y < 0)
                {
                    spriteRB.gravityScale = fallingGravity;
                    detectBase();
                }
                vMove = 0;

            }

            Vector3 targetVelocity = new Vector2(hMove * hSpeed * Time.deltaTime, vMove * vSpeed * Time.deltaTime);
            targetVelocity.z = 0;

            trueRB.velocity = Vector3.ClampMagnitude(targetVelocity, clampValue);

            Vector3 newPos = spriteRB.transform.position;
            newPos.x = trueRB.transform.position.x;

            if (grounded)
            {
                newPos.y = trueRB.transform.position.y;
            }

            newPos.z = 0;
            spriteRB.transform.position = newPos;

            if (jump && grounded)
            {
                Debug.Log("Jump");
                spriteRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                spriteRB.gravityScale = risingGravity;
                grounded = false;
            }

            if (hMove > 0 && !faceRight)
            {
                Flip();
            }
            else if (hMove < 0 && faceRight)
            {
                Flip();
            }
        }
    }

    public void Attack()
    {
        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            Debug.Log("Attack");
            meleeStateMachine.SetNextState(new MeleeEntryState());
        }

    }

    public void Flip()
    {
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
    }

    private void detectBase()
    {
        RaycastHit2D hit = Physics2D.Raycast(spriteRB.transform.position, Vector2.down, detectionDistance, detectLayer);
        if (hit.collider != null)
        {
            Debug.Log("Base Deteced");
            grounded = true;
            spriteRB.velocity = new Vector2(spriteRB.velocity.x, 0);
            spriteRB.gravityScale = 0;
        }
    }

    protected void ZeroAllVelocity()
    {
        trueRB.velocity = Vector3.zero;
        spriteRB.velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(spriteRB.transform.position, Vector3.down * detectionDistance);
    }

}
