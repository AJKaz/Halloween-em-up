using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMeleeState : MeleeBaseState
{


    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);

        attackIndex = 1;
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
                stateMachine.SetNextState(new GroundMeleeState2());
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
