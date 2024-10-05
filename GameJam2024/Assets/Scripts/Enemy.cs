using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private bool bAttacking = false;

    [SerializeField]
    private float runAwaySpeedMultiplier = 1.5f;
    [SerializeField]
    private float attackRange = 1.25f;

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
        direction.Normalize();

        Move(direction.x, direction.y, false);
        
    }

    void MoveTo(Vector2 direction, float speed) {
        float scaledSpeed = Time.deltaTime * speed;

        Vector3 newPos = transform.position;
        newPos.x += direction.x * scaledSpeed;
        newPos.y += direction.y * scaledSpeed;
        transform.position = newPos;
    }

    void RunAway() {
        // TODO: Make direciton dynamic
        // IF on left half of screen, run left
        // IF on right half of screen, run right
        // Can have some verticality to this to make it unique, but make it slight
        // Note: It's okay if this is run every frame, technically worse for performance but ok for this scale
        Vector2 direction = new Vector2(-1, 0);
        direction.Normalize();
        //MoveTo(direction, runAwaySpeedMultiplier);
        Move(direction.x, direction.y, false);
    }

    bool InAttackRange() {
        return ((GameManager.Instance.player.transform.position - transform.position).magnitude < attackRange);
    }

    void OnStunned() {
        bAttacking = false;
    }
}
