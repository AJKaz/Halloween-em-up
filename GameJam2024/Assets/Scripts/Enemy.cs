using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    private bool bAttacking = false;

    [SerializeField] private float health = 100f;

    [SerializeField] private float runAwaySpeedMultiplier = 1.5f;
    [SerializeField] private float attackRange = 1.25f;

    public string enemyName = "No Name";

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
           // ZeroAllVelocity();

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
        if (direction.magnitude > attackRange) {
            direction.Normalize();
            
            //Move(direction.x, direction.y, false);
        }
        else {
          // ZeroAllVelocity(); 
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
        return ((GameManager.Instance.player.transform.position - transform.position).magnitude < attackRange);
    }

    void OnStunned() {
        bAttacking = false;
    }
}
