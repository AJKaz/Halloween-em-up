using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : MeleeBaseState
{
    private bool grounded;
    public void Awake()
    {
       

    }

   public override void OnEnter(StateMachine sMachine)
   {
        base.OnEnter(sMachine);
        
        grounded = GetComponent<CharacterMovement>().grounded;
        
        if (grounded)
        {
            sMachine.SetNextState(new GroundMeleeState());
        }
        else
        {
            sMachine.SetNextState(new AirMeleeState());
        }
   }
}
