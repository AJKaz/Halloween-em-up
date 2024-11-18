using UnityEngine;

public class EnemyPathState : EnemyState {
    
    public EnemyPathState(Enemy e) : base(e) { }

    public override void OnEnter() {
        base.OnEnter();
    }

    public override void OnUpdate() {
        base.OnUpdate();

 
        PathToPosition(GameManager.Instance.player.transform.position + Separate());
    }

    private Vector3 Separate() {
        Vector3 separation = Vector3.zero;
        
        foreach (Enemy otherEnemy in GameManager.Instance.enemies) {
            if (otherEnemy == enemy) continue;

            Vector3 toOtherEnemy = enemy.transform.position - otherEnemy.transform.position;
            float distance = toOtherEnemy.magnitude;

            if (distance < enemy.separateThreshold) {
                separation += toOtherEnemy / distance;  // closer the enemy is, the more they'll separate
            }

        }

        return separation.normalized;
    }

    private void PathToPosition(Vector3 position) {
        Vector2 direction = position - enemy.transform.position;
        Vector2 distance = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        float hActualRange = enemy.hAttackRange + enemy.hAttackOffset;
        
        if (distance.x > hActualRange || distance.y > enemy.verticalPathingOffset) {
            if (distance.x < hActualRange) direction.x = 0;
            if (distance.y < enemy.verticalPathingOffset) direction.y = 0;
            else direction.y *= enemy.vPriority;
            direction.Normalize();

            Move(direction.x, direction.y);
        }
        else {
            enemy.rb.velocity = Vector3.zero;
        }
    }

    private void Move(float hMove, float vMove) {
        if (hMove < 0 && enemy.bFacingRight) enemy.Flip();
        else if (hMove > 0 && !enemy.bFacingRight) enemy.Flip();

        Vector3 targetVelocity = new Vector2(hMove * enemy.HSpeed, vMove * enemy.VSpeed);
        targetVelocity.z = 0;

        enemy.rb.velocity = targetVelocity;
    }

}
