using UnityEngine;

public class EnemyAttackState : EnemyState
{
    protected Animator animator;

    public bool bAttacking;

    public EnemyAttackState(Enemy e) : base(e) { }

    public override void OnEnter() {
        base.OnEnter();
        
        bAttacking = true;

        if (animator) {
            animator.SetTrigger("Attack1");
        }
    }
    public void SetAnimator(Animator a) { animator = a; }

    public override void OnUpdate() {
        base.OnUpdate();

        if (time > enemy.attackDuration) {
            enemy.ReturnToDefaultState();
        }
    }

    public override void OnExit() { 
        base.OnExit();

        bAttacking = false;
    }
   
}
