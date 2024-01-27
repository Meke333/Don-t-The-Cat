using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Timer
{
    private float _currentTime;
    private int _timerTimeInMilliSeconds;
    public Action onTimerDone;

    public void SetTimer(int newTime)
    {
        _timerTimeInMilliSeconds = newTime;
    }

    async public void RunTimer()
    {
        await Task.Delay(_timerTimeInMilliSeconds);
        onTimerDone?.Invoke();
    }

}
