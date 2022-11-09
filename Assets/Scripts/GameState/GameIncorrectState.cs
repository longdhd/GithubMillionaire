using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIncorrectState : GameBaseState
{
    int animIDIncorrect = Animator.StringToHash("Incorrect");

    public override void EnterState(GameStateManager state)
    {
        Animator animator = state.GetComponent<Animator>();
        animator.SetTrigger(animIDIncorrect);
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {

    }
}
