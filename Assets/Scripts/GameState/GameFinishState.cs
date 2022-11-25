using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishState : GameBaseState
{
    public override void EnterState(GameStateManager state)
    {
        CameraManager.Instance.SwitchTo("OverviewCamera");

        state.PlayerAnimator.SetTrigger("Finish");

        SoundManager.Instance.PlayMusic(state.FinishAudioClip);

        foreach(var obj in state.objsToHide)
        {
            obj.SetActive(false);
        }
    }

    public override void UpdateState(GameStateManager state)
    {
        
    }

    public override void ExitState(GameStateManager state)
    {
        foreach (var obj in state.objsToHide)
        {
            obj.SetActive(true);
        }

        state.LastQuestion = false;
    }
}
