using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCorrectState : GameBaseState
{
    float? timer = null;
    public override void EnterState(GameStateManager state)
    {
        if (!state.LastQuestion)
        {
            int random = Random.Range(1, 3);
            state.PlayerAnimator.SetTrigger(random == 1 ? state.AnimIDCorrect1 : state.AnimIDCorrect2);
            state.HostAnimator.SetTrigger(random == 1 ? "Correct1" : "Correct2");

            SoundManager.Instance.PlayMusic(state.CorrectAudioClip);
        }

        int ramdomClip = Random.Range(0, state.CorrectEffectClip.Length);
        SoundManager.Instance.PlayEffect(state.CorrectEffectClip[ramdomClip]);

        timer = 0;

        LightAnimationManager.Instance.ChangeLightColor(false);
    }
    public override void UpdateState(GameStateManager state)
    {
        timer += Time.deltaTime;
        if (!state.LastQuestion && timer >= 5f)
        {
            CameraManager.Instance.SwitchTo("HostCam");
        }

        LightAnimationManager.Instance.RotateDownLightOff();
    }
    public override void ExitState(GameStateManager state)
    {
        SoundManager.Instance.UnloadMusic(state.CorrectAudioClip);
    }
}
