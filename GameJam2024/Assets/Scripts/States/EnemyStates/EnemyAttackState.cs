using UnityEngine;

public class EnemyAttackState : EnemyState
{
    protected Animator animator;

    public bool bAttacking;

    public EnemyAttackState(Enemy e) : base(e) { }

    public override void OnEnter() {
        base.OnEnter();
        bAttacking = false;
    }
    public void SetAnimator(Animator a) { animator = a; }

    public override void OnUpdate() {
        base.OnUpdate();

        bAttacking = true;

        if (time > enemy.attackDuration) {
            enemy.ReturnToDefaultState();
        }

        if (animator) {
            animator.SetTrigger("Attack1");
        }

    }

    public override void OnExit() { 
        base.OnExit();

        bAttacking = false;
    }
   
}
