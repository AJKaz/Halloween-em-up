using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State
{
    // How long the state is
    public float duration;
   
    protected Animator animator;

    // if an attack is requested
    protected bool attackSignal;

    //decide if next move should come out 
    protected bool nextCombo;

    //decide if launch should come out
    protected bool launch = false;

    protected int attackIndex;


    public override void OnEnter(StateMachine sMachine)
    {
        base.OnEnter(sMachine);
        animator = GetComponent<Animator>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.U))
        {
            nextCombo = true;
        }
    }

    
}
