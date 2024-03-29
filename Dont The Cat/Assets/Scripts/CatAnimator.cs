using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    private CatScript _catScript;
    
    public Animator catAnimator;

    struct Names
    {
        public static readonly int Location = Animator.StringToHash("Location");
        public static readonly int IntoLocationMovement = Animator.StringToHash("IntoLocationMovement");
        public static readonly int EnterToAnticipation = Animator.StringToHash("EnterToAnticipation");
        public static readonly int AnticipationToTheIncident = Animator.StringToHash("AnticipationToTheIncident");
        public static readonly int IsPetting = Animator.StringToHash("IsPetting");
        public static readonly int InPetState = Animator.StringToHash("InPetState");
        public static readonly int IsJumping = Animator.StringToHash("IsJumping");
        public static readonly int IsOverpet = Animator.StringToHash("IsOverpet");
        public static readonly int PetTimerDone = Animator.StringToHash("PetTimerDone");
        public static readonly int HasLanded = Animator.StringToHash("HasLanded");
        public static readonly int CatMeter = Animator.StringToHash("CatMeter");
    }

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
    }

    private async void OnEnable()
    {
        //_catScript.onCatLocationChange += ProcessAction_OnCatLocation;
        _catScript.onCatStateChange += ProcessAction_OnCatState;
        _catScript.onCatJump += ProcessAction_OnCatJump;
        _catScript.onCatLanded += ProcessAction_OnCatLanded;
        await Task.Yield();
        GameEventManager.Instance.onCatLocationSet += ProcessAction_OnCatLocation;
        GameEventManager.Instance.onTransmittingPetValue += ProcessAction_onTransmittingPetValue;
        GameEventManager.Instance.onCatInPetState += ProcessAction_OnCatInPetState;
        GameEventManager.Instance.onCatGettingPet += ProcessAction_OnCatGettingPet;
        GameEventManager.Instance.onYouHaveNotPleasedTheCatInTime += ProcessAction_onYouHaveNotPleasedTheCatInTime;
        GameEventManager.Instance.onCatReaction += StopPet;
    }

    private void OnDisable()
    {
        //_catScript.onCatLocationChange -= ProcessAction_OnCatLocation;
        _catScript.onCatJump -= ProcessAction_OnCatJump;
        _catScript.onCatStateChange -= ProcessAction_OnCatState;
        _catScript.onCatLanded -= ProcessAction_OnCatLanded;
        GameEventManager.Instance.onCatLocationSet -= ProcessAction_OnCatLocation;
        GameEventManager.Instance.onCatInPetState -= ProcessAction_OnCatInPetState;
        GameEventManager.Instance.onCatGettingPet -= ProcessAction_OnCatGettingPet;
        GameEventManager.Instance.onYouHaveNotPleasedTheCatInTime -= ProcessAction_onYouHaveNotPleasedTheCatInTime;
        GameEventManager.Instance.onCatReaction -= StopPet;
    }

    #region EventMethods

    void ProcessAction_OnCatLocation(CatLocation location)
    {
        AudioHandler.Instance.PlayLoopSound(Clip.Cat_Walking_On_Wood);

        switch (location)
        {
            case CatLocation.Nothing:
                break;
            case CatLocation.Vase:
                catAnimator.SetInteger(Names.Location, 1);
                catAnimator.SetTrigger(Names.IntoLocationMovement);
                break;
            case CatLocation.Urne:
                catAnimator.SetInteger(Names.Location, 2);
                catAnimator.SetTrigger(Names.IntoLocationMovement);
                break;
            case CatLocation.Radio:
                catAnimator.SetInteger(Names.Location, 3);
                catAnimator.SetTrigger(Names.IntoLocationMovement);
                break;
            case CatLocation.SelfDestructButton:
                catAnimator.SetInteger(Names.Location, 4);
                catAnimator.SetTrigger(Names.IntoLocationMovement);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }
    }

    void ProcessAction_OnCatState(CatState state)
    {
        switch(state)
        {
            case CatState.Unpetted:
                break;
            case CatState.InPetMode:
                catAnimator.SetBool(Names.InPetState, true);
                break;
            case CatState.Pleased:
                catAnimator.SetBool(Names.InPetState, false);
                break;
            case CatState.Overpetted:
                catAnimator.SetBool(Names.InPetState, false);
                break;
            case CatState.UnderPetted:
                catAnimator.SetBool(Names.InPetState, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void ProcessAction_OnCatInPetState()
    {
        catAnimator.SetTrigger(Names.InPetState);
    }

    void ProcessAction_OnCatGettingPet(bool isPet)
    {
        catAnimator.SetBool(Names.IsPetting, isPet);
    }

    void ProcessAction_OnCatJump()
    {
        AudioHandler.Instance.StopLoopSound();
        catAnimator.SetTrigger(Names.IsJumping);
    }

    async void ProcessAction_OnCatLanded()
    {
        AudioHandler.Instance.PlaySingleSound(Clip.Landing_Sound);
        catAnimator.SetTrigger(Names.HasLanded);

        await Task.Delay(800);

        AudioHandler.Instance.PlaySingleSound(Clip.Paper_Rustle);
    }

    void ProcessAction_onTransmittingPetValue(double value)
    {
        catAnimator.SetFloat(Names.CatMeter, (float) value);
    }

    void ProcessAction_onYouHaveNotPleasedTheCatInTime()
    {
        catAnimator.SetTrigger(Names.AnticipationToTheIncident);
    }

    void StopPet(CatState a)
    {
        catAnimator.SetTrigger(Names.PetTimerDone);
        catAnimator.SetBool(Names.IsPetting, false);
    }
    

    #endregion
}
