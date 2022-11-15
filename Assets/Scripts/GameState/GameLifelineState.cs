using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLifelineState : GameBaseState
{
    public override void EnterState(GameStateManager state)
    {
        state.PlayerAnimator.SetTrigger(state.AnimIDLifeline);
        state.HostAnimator.SetTrigger("Lifeline");

    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {
        state.PlayerAnimator.ResetTrigger(state.AnimIDLifeline);
        state.HostAnimator.ResetTrigger(state.AnimIDLifeline);
    }
}
