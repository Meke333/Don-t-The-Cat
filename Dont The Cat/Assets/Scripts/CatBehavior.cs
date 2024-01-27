using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatBehavior : MonoBehaviour
{
    public CatLocation location = CatLocation.Nothing;
    public CatLocation nextLocation;
    
    public CatState state = CatState.Normal;
    public CatState nextState;
    
    public StateStage stage = StateStage.Enter;

    private Timer _timer;
    public bool isTimerDone;
    
    public int reactionTime;
    public int catMinNothingTime;
    public int catMaxNothingTime;


    private void OnEnable()
    {
        _timer.onTimerDone += ProcessAction_TimerDone;
    }

    private void OnDisable()
    {
        _timer.onTimerDone -= ProcessAction_TimerDone;
    }

    private void Update()
    {
        ProcessStage();
    }

    #region Methods

    void ProcessState()
    {
        switch (location)
        {
            case CatLocation.Nothing:
                Nothing_Location();
                break;
            case CatLocation.Vase:
                Vase_Location();
                break;
            case CatLocation.Urne:
                Urne_Location();
                break;
            case CatLocation.Radio:
                Radio_Location();
                break;
            case CatLocation.SelfDestructButton:
                SelfDestructButton_Location();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void ProcessStage()
    {
        ProcessState();
        
        switch (stage)
        {
            case StateStage.Enter:
                stage = StateStage.Update;
                break;
            case StateStage.Update:
                break;
            case StateStage.Exit:
                break;
        }
    }
    

    #region CatLocations

    void Nothing_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                _timer.SetTimer(Random.Range(catMinNothingTime, catMaxNothingTime));
                isTimerDone = false;
                _timer.RunTimer();
                break;
            case StateStage.Update:
                if (!isTimerDone)
                    return;

                int randomLocation = Random.Range(1, Enum.GetNames(typeof(CatLocation)).Length);
                nextLocation = (CatLocation)randomLocation;
                
                stage = StateStage.Exit;
                
                break;
            case StateStage.Exit:
                break;
        }
    }

    void Vase_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                SetDangerTimer();
                break;
            case StateStage.Update:
                //if Timer is done => You Dead
                //if 
                break;
            case StateStage.Exit:
                
                break;
        }
    }

    void Urne_Location()
    {
        
    }

    void Radio_Location()
    {
        
    }

    void SelfDestructButton_Location()
    {
        
    }

    void SetDangerTimer()
    {
        _timer.SetTimer(reactionTime);
        isTimerDone = false;
        _timer.RunTimer();
    }

    #endregion
    

    #endregion

    #region EventMethods

    void ProcessAction_TimerDone()
    {
        isTimerDone = true;
    }

    #endregion
}

public enum CatLocation
{
    Nothing,
    Vase,
    Urne,
    Radio,
    SelfDestructButton
}

public enum CatState
{
    Normal,
    Unpetted,
    Pleased,
    Overpetted
}

public enum StateStage
{
    Enter,
    Update,
    Exit,
}
