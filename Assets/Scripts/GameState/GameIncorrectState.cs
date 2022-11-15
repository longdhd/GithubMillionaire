using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIncorrectState : GameBaseState
{
    public override void EnterState(GameStateManager state)
    {
        state.PlayerAnimator.SetTrigger(state.AnimIDIncorrect);
        state.HostAnimator.SetTrigger("Incorrect");

        SoundManager.Instance.PlayMusic(state.IncorrectAudioClip);
        int ramdomClip = Random.Range(0, state.IncorrectEffectClip.Length);
        SoundManager.Instance.PlayEffect(state.IncorrectEffectClip[ramdomClip]);
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {

    }
}
