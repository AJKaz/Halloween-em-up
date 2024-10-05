using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMeleeState3 : MeleeBaseState
{
    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);

        attackIndex = 3;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " fired");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedTime >= duration)
        {
            if (launch)
            {
                stateMachine.SetNextState(new LaunchMeleeState());
            }
        }
    }
}