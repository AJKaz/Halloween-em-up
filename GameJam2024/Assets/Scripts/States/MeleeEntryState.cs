using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
        
        grounded = GetComponent<Character>().grounded;
        Debug.Log(grounded);
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
