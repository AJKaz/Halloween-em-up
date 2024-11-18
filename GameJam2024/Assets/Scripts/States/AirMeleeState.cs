using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMeleeState : MeleeBaseState
{
    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);

        attackIndex = 5;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (fixedTime >= duration)
        {
           
            stateMachine.SetNextStateToMain();
            
        }
    }
}
