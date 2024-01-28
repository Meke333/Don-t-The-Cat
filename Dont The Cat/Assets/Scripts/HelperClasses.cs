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

    private bool _isTimerInterrupted;

    public void SetTimer(int newTime)
    {
        _isTimerInterrupted = false;
        _timerTimeInMilliSeconds = newTime;
    }

    async public void RunTimer()
    {
        
        await Task.Delay(_timerTimeInMilliSeconds);
        if (_isTimerInterrupted) 
            return; 
        
        onTimerDone?.Invoke();
    }
    
    public void InterruptTimer()
    {
        _isTimerInterrupted = true;
    }

}
