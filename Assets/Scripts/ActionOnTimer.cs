using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionOnTimer : MonoBehaviour
{
    DateTime dt;
    float timer;
    float remainingTime;
    bool isRunning = false;
    Action timerCallback;

    public void SetTimer(float timer, Action timerCallback)
    {
        this.timer = timer;
        this.timerCallback = timerCallback;
        isRunning = true;
        dt = DateTime.Now;
    }

    void Update()
    {
        if (timer > 0 && isRunning)
        {
            remainingTime = timer - (float)(DateTime.Now - dt).TotalSeconds;
            if (IsOutOfTime())
            {
                isRunning = false;
                timerCallback();
            }
        }
    }

    bool IsOutOfTime()
    {
        return isRunning && remainingTime <= 0;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public int GetRemainingTime()
    {
        //if (!isRunning) return 0;

        return (int)Mathf.Round(remainingTime);
    }
}
