using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIdleState : GameBaseState
{
    AudioClip lastClip;
    AudioClip currentClip;
    public override void EnterState(GameStateManager state)
    {
        AudioClip clip = RandomClip(state.IdleAudioClip);
        currentClip = clip;
        SoundManager.Instance.PlayMusic(clip);

        //CameraManager.Instance.SwitchTo("PlayerCam");
    }
    public override void UpdateState(GameStateManager state)
    {

    }
    public override void ExitState(GameStateManager state)
    {
        SoundManager.Instance.UnloadMusic(currentClip);
    }
    AudioClip RandomClip(AudioClip[] audioClipArray)
    {
        int attempts = 3;
        AudioClip newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        while (newClip == lastClip && attempts > 0)
        {
            newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
            attempts--;
        }
        lastClip = newClip; 
        return newClip;
    }
}
