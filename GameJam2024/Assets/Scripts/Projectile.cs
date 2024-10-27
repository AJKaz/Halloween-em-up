using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private float lifetime = 0.75f;
    [SerializeField] private float collisionOffset = 0.35f;


    private Vector2 direction;
    private float timeAlive = 0f;
    private bool bShootingRight = false;
    public void SetDirection(bool bFacingRight) {
        bShootingRight = bFacingRight;
        direction = bFacingRight ? Vector2.right : Vector2.left;
    }

    private void Update() {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifetime) {
            Destroy(gameObject);
            return;
        }

        Vector3 scaledDirection = direction * Time.deltaTime * speed;
        transform.position = transform.position + scaledDirection;
        transform.Rotate(new Vector3(0, 0, rotateSpeed));

        ManualPlayerCollisionCheck();
    }

    private void ManualPlayerCollisionCheck() {
        // Gross
        // TODO: Should use collider on player, but giving player collider stops them from jumping
        // didn't have time to look into that, so did this for time being
        
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        
        if (bShootingRight && transform.position.x > playerPos.x - collisionOffset) {
            Destroy(gameObject);
        }
        else if (!bShootingRight && transform.position.x < playerPos.x + collisionOffset) {
            Destroy(gameObject);
        }
    }
}
