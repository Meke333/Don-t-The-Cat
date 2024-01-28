using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CatBehavior : MonoBehaviour
{
    private CatScript _catScript;
    
    public CatLocation location = CatLocation.Nothing;
    public CatLocation nextLocation;
    
    public CatState state = CatState.Unpetted;
    
    public StateStage stage = StateStage.Enter;

    private Timer _nothingTimer = new Timer();
    public bool isNothingTimerDone;
    
    private Timer _reactionTimer = new Timer();
    public bool isReactionTimerDone;

    [Space]
    [Header("GAME DESIGN")]
    
    public int reactionTime;
    public int catMinNothingTime;
    public int catMaxNothingTime;

    [Range(0,1)]
    public double underpettedLimit;
    [Range(0,1)]
    public double overpettedLimit, absolutelyOverpettedLimit;

    public bool wasVaseElimintaed, wasUrneElimintaed, wasRadioEliminated, wasSelftdestructionButtonEliminated;

    public bool isCatReactionPlaying;

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
        wasRadioEliminated = false;
        wasSelftdestructionButtonEliminated = false;
        wasUrneElimintaed = false;
        wasVaseElimintaed = false;

        isCatReactionPlaying = false;
    }

    async private void OnEnable()
    {
        _nothingTimer.onTimerDone += () => isNothingTimerDone = true;
        _reactionTimer.onTimerDone += ProcessAction_TimerDone;
        _catScript.onCatStateChange += ProcessAction_CatStateChange;
        _catScript.onCatLanded += SetDangerTimer;
        await Task.Yield();
        GameEventManager.Instance.onCatInteraction += ProcessAction_CatInteraction;
        GameEventManager.Instance.onCatInPetState += ProcessAction_OnCatInPetState;
        GameEventManager.Instance.onTransmittingPetValue += ProcessAction_onTransmittingPetValue;
    }

    private void OnDisable()
    {
        _nothingTimer.onTimerDone -= () => isNothingTimerDone = true;
        _reactionTimer.onTimerDone -= ProcessAction_TimerDone;
        _catScript.onCatStateChange -= ProcessAction_CatStateChange;
        _catScript.onCatLanded -= SetDangerTimer;
        GameEventManager.Instance.onCatInteraction -= ProcessAction_CatInteraction;
        GameEventManager.Instance.onCatInPetState -= ProcessAction_OnCatInPetState;
        GameEventManager.Instance.onTransmittingPetValue -= ProcessAction_onTransmittingPetValue;
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
                GameEventManager.Instance.onCatLocationSet?.Invoke(location);
                stage = StateStage.Enter;
                break;
        }
        
    }
    

    #region CatLocations

    void Nothing_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                _nothingTimer.SetTimer(Random.Range(catMinNothingTime, catMaxNothingTime));
                isNothingTimerDone = false;
                _nothingTimer.RunTimer();
                break;
            case StateStage.Update:
                if (!isNothingTimerDone)
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
                break;
            case StateStage.Update:
                //if Timer is done => You Dead
                if (!isReactionTimerDone)
                    return;

                if (state == CatState.InPetMode)
                    return;

                if (!wasVaseElimintaed)
                {
                    wasVaseElimintaed = true;
                    AudioHandler.Instance.PlaySingleSound(Clip.Glass_Shatter);
                }

                //DEAD
                GameEventManager.Instance.onPlayerDied?.Invoke();
                
                break;
            case StateStage.Exit:
                break;
        }
    }

    void Urne_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                break;
            case StateStage.Update:
                //if Timer is done => You Dead
                if (!isReactionTimerDone)
                    return;
                
                if (state == CatState.InPetMode)
                    return;

                if (!wasUrneElimintaed)
                {
                    wasUrneElimintaed = true;
                    AudioHandler.Instance.PlaySingleSound(Clip.Glass_Shatter);
                }

                //DEAD
                GameEventManager.Instance.onYouHaveNotPleasedTheCatInTime?.Invoke();
                GameEventManager.Instance.onPlayerDied?.Invoke();
                
                break;
            case StateStage.Exit:
                break;
        }
    }

    void Radio_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                break;
            case StateStage.Update:
                //if Timer is done => You Dead
                if (!isReactionTimerDone)
                    return;
                
                if (state == CatState.InPetMode)
                    return;

                if (!wasRadioEliminated)
                {
                    wasRadioEliminated = true;
                    AudioHandler.Instance.PlaySingleSound(Clip.Cat_Exploding);
                }

                
                //DEAD
                GameEventManager.Instance.onPlayerDied?.Invoke();
                
                break;
            case StateStage.Exit:
                break;
        }
    }

    void SelfDestructButton_Location()
    {
        switch (stage)
        {
            case StateStage.Enter:
                break;
            case StateStage.Update:
                //if Timer is done => You Dead
                if (!isReactionTimerDone)
                    return;
                
                if (state == CatState.InPetMode)
                    return;

                if (!wasSelftdestructionButtonEliminated)
                {
                    wasSelftdestructionButtonEliminated = true;
                    AudioHandler.Instance.PlaySingleSound(Clip.Selfdestruction_Siren);
                }


                //DEAD
                GameEventManager.Instance.onPlayerDied?.Invoke();
                
                break;
            case StateStage.Exit:
                break;
        }
    }

    void SetDangerTimer()
    {
        Debug.Log("CAT LANDED");
        
        _reactionTimer.SetTimer(reactionTime);
        isReactionTimerDone = false;
        _reactionTimer.RunTimer();

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
        isReactionTimerDone = true;
    }

    void ProcessAction_CatStateChange(CatState newState)
    {
        state = newState;

        switch (state)
        {
            case CatState.Unpetted:
                break;
            case CatState.InPetMode:
                _reactionTimer.InterruptTimer();
                break;
            case CatState.Pleased:
                nextLocation = CatLocation.Nothing;
                state = CatState.Unpetted;
                stage = StateStage.Exit;
                ProcessStage();
                break;
            case CatState.Overpetted:
                AudioHandler.Instance.PlaySingleSound(Clip.Gun_Shot);
                break;
            case CatState.UnderPetted:
                //YOU DIED
                break;
        }
    }

    void ProcessAction_CatInteraction(double value)
    {
        
        CatState newCatState = 
            (value < underpettedLimit)? CatState.UnderPetted : 
            (value > overpettedLimit) ? CatState.Overpetted : CatState.Pleased;
        _catScript.onCatStateChange?.Invoke(newCatState);
        GameEventManager.Instance.onCatReaction?.Invoke(newCatState);
        
        Debug.Log("CAT INTERACTION WEWO: " + newCatState);
    }

    async void ProcessAction_onTransmittingPetValue(double value)
    {
        if (isCatReactionPlaying)
            return;

        isCatReactionPlaying = true;

        if (value < underpettedLimit)
            AudioHandler.Instance.PlaySingleSound(Clip.Calm_Pet_Demanding); //demanding
        else if (value > overpettedLimit)
        {
            AudioHandler.Instance.PlaySingleSound(Random.Range(0, 7) > 5 ? Clip.Agressive_Hissing : Clip.Aggressive_Meow); //1/4 hissing, 3/4 meowing
        } else if(value > absolutelyOverpettedLimit)
        {
            AudioHandler.Instance.PlaySingleSound(Clip.Aggressive_Scream);
        }
        else
            AudioHandler.Instance.PlaySingleSound(Random.Range(0, 3) > 2 ? Clip.Calm_Pur : Clip.Calm_Meow); //1/3 puring, 2/3 meowing

        await Task.Delay(800);

        isCatReactionPlaying = false;
    }

    private void ProcessAction_OnCatInPetState()
    {
        _reactionTimer.InterruptTimer();
    }

    #endregion
}


public enum StateStage
{
    Enter,
    Update,
    Exit,
}
