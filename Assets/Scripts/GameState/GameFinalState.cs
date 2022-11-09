using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinalState : GameBaseState
{
    int animIDFinal = Animator.StringToHash("Final");
    public override void EnterState(GameStateManager state)
    {
        Animator animator = state.GetComponent<Animator>();
        animator.SetTrigger(animIDFinal);
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {

    }
}
