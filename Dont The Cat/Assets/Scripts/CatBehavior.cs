using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CatBehavior : MonoBehaviour
{
    private CatScript _catScript;
    
    public CatLocation location = CatLocation.Nothing;
    public CatLocation nextLocation;
    
    public CatState state = CatState.Unpetted;
    public CatState nextState;
    
    public StateStage stage = StateStage.Enter;

    private Timer _timer = new Timer();
    public bool isTimerDone;

    public bool isPleased;

    public int reactionTime;
    public int catMinNothingTime;
    public int catMaxNothingTime;

    [Range(0,1)]
    public double underpettedLimit;
    [Range(0,1)]
    public double overpettedLimit;

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
    }

    private void OnEnable()
    {
        _timer.onTimerDone += ProcessAction_TimerDone;
        _catScript.onCatStateChange += ProcessAction_CatStateChange;
        GameEventManager.Instance.onCatInteraction += ProcessAction_CatInteraction;
    }

    private void OnDisable()
    {
        _timer.onTimerDone -= ProcessAction_TimerDone;
        _catScript.onCatStateChange -= ProcessAction_CatStateChange;
        GameEventManager.Instance.onCatInteraction -= ProcessAction_CatInteraction;
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
                location = nextLocation;
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
                stage = StateStage.Enter;
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
                if (!isTimerDone)
                    return;
                
                if (state != CatState.InPetMode)
                    return;
                
                //DEAD
                GameEventManager.Instance.onPlayerDied?.Invoke();
                
                break;
            case StateStage.Exit:
                stage = StateStage.Enter;
                break;
        }
    }

    void Urne_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                SetDangerTimer();
                break;
            case StateStage.Update:
                break;
            case StateStage.Exit:
                stage = StateStage.Enter;
                break;
        }
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

        nextLocation = CatLocation.Nothing;
    }

    void PlayerDied()
    {
        
    }

    #endregion
    

    #endregion

    #region EventMethods

    void ProcessAction_TimerDone()
    {
        isTimerDone = true;
    }

    void ProcessAction_CatStateChange(CatState newState)
    {
        state = newState;

        switch (state)
        {
            case CatState.Unpetted:
                break;
            case CatState.InPetMode:
                _timer.InterruptTimer();
                break;
            case CatState.Pleased:
                stage = StateStage.Exit;
                break;
            case CatState.Overpetted:
                //YOU DIED
                break;
            case CatState.UnderPetted:
                //YOU DIED
                break;
        }
    }

    void ProcessAction_CatInteraction(double value)
    {
        CatState newCatState = 
            (value < underpettedLimit)? CatState.Unpetted : 
            (value > overpettedLimit) ? CatState.Overpetted : CatState.Pleased;
        _catScript.onCatStateChange?.Invoke(newCatState);
        GameEventManager.Instance.onCatReaction?.Invoke(newCatState);
    }

    #endregion
}


public enum StateStage
{
    Enter,
    Update,
    Exit,
}
