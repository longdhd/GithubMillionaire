using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLifelineState : GameBaseState
{
    int animIDLifeline = Animator.StringToHash("Lifeline");
    public override void EnterState(GameStateManager state)
    {
        Animator animator = state.GetComponent<Animator>();
        animator.SetTrigger(animIDLifeline);
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {
        Animator animator = state.GetComponent<Animator>();
        animator.ResetTrigger(animIDLifeline);
    }
}
