using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCorrectState : GameBaseState
{
    int animIDCorrect1 = Animator.StringToHash("Correct1");
    int animIDCorrect2 = Animator.StringToHash("Correct2");
    public override void EnterState(GameStateManager state)
    {
        int random = Random.Range(1, 3);
        Animator animator = state.GetComponent<Animator>();
        if (random == 1)
            animator.SetTrigger(animIDCorrect1);
        else
            animator.SetTrigger(animIDCorrect2);
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {

    }
}
