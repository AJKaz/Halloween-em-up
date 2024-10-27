using UnityEngine;

public class EnemyAttackState : EnemyState
{
    protected Animator animator;

    public bool bAttacking;

    public EnemyAttackState(Enemy e) : base(e) { }

    public override void OnEnter() {
        base.OnEnter();
        
        bAttacking = true;

        // TEMP: ZOMBIE doesn't have attack anim (ATM), so need this
        // TODO: Once attack anim is added, can remove this and 
        // bind attack damage via animation frame event, like witch has
        if (enemy.enemyType == EnemyType.Zombie) {
            enemy.TryDamagePlayer();
            return;
        }

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
