using UnityEngine;

public class LaunchMeleeState : MeleeBaseState
{
    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);

        attackIndex = 4;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedTime >= duration)
        {
            if (nextCombo)
            {
                stateMachine.SetNextState(new AirMeleeState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
