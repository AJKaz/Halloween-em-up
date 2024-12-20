using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMeleeState2 : MeleeBaseState
{
    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);

        attackIndex = 2;
        duration = 0.7f;
        animator.SetTrigger("Attack" + attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedTime >= duration)
        {
            if (nextCombo)
            {
                stateMachine.SetNextState(new GroundMeleeState3());
            }
            else if (launch)
            {
                stateMachine.SetNextState(new LaunchMeleeState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
