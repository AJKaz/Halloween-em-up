using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float health = 500f;

    [Header("Ground")]
    [SerializeField] private float hSpeed = 5000f;
    [SerializeField] private float vSpeed = 3500f;
    [SerializeField] private Rigidbody2D trueRB;
    [SerializeField] private float clampValue = 5;
    [SerializeField] private float offset = 1.05f;
    private bool faceRight = true;

    [Header("Jump")]
    [SerializeField] private Rigidbody2D spriteRB;
    [SerializeField] float jumpForce = 7f;
    private bool grounded = true;
    [SerializeField] private float detectionDistance = 1.05f;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private float risingGravity = 1;
    [SerializeField] private float fallingGravity = 1.5f;

    public void Move(float hMove, float vMove, bool jump)
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

        trueRB.velocity = Vector3.ClampMagnitude(targetVelocity,clampValue);

        Vector3 newPos = spriteRB.transform.position;
        newPos.x = trueRB.transform.position.x;

        if (grounded)
        {
            newPos.y = trueRB.transform.position.y + offset;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(spriteRB.transform.position, Vector3.down * detectionDistance);
    }
}
