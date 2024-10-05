using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MeleeEntryState : MeleeBaseState
{
    private bool grounded;
    public void Awake()
    {
        grounded = GetComponent<Character>().grounded;
    }

   public override void OnEnter(StateMachine sMachine)
   {
        base.OnEnter(sMachine);

        if (grounded)
        {
            State nextState = (State)new GroundMeleeState();
        }
        else
        {
            State nextState = (State)new AirMeleeState();
        }
   }
}
