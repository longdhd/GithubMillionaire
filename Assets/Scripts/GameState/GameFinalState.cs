using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinalState : GameBaseState
{
    public override void EnterState(GameStateManager state)
    {
        state.PlayerAnimator.SetTrigger(state.AnimIDFinal);
        state.HostAnimator.SetTrigger("Final");

        CameraManager.Instance.SwitchTo("OverviewCam");
        SoundManager.Instance.PlayEffect(state.FinalEffectClip);
    }
    public override void UpdateState(GameStateManager state)
    {
        LightAnimationManager.Instance.RotateUpLightOn();
    }
    public override void ExitState(GameStateManager state)
    {

    }
}
