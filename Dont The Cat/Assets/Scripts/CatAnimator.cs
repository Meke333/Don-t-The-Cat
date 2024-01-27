using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    private CatScript _catScript;
    
    public Animator catAnimator;

    struct AnimatorVariables
    {
        private static readonly int Location = Animator.StringToHash("Location");
        private static readonly int IntoLocationMovement = Animator.StringToHash("IntoLocationMovement");
        private static readonly int EnterToAnticipation = Animator.StringToHash("EnterToAnticipation");
        private static readonly int AnticipationToTheIncident = Animator.StringToHash("AnticipationToTheIncident");
        private static readonly int IsPetting = Animator.StringToHash("IsPetting");
        private static readonly int InPetState = Animator.StringToHash("InPetState");
        private static readonly int IsOverpet = Animator.StringToHash("IsOverpet");
        private static readonly int PetTimerDone = Animator.StringToHash("PetTimerDone");
        private static readonly int HasLanded = Animator.StringToHash("HasLanded");
    }

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
    }

    private void OnEnable()
    {
        _catScript.onCatLocationChange += ProcessAction_OnCatLocation;
        _catScript.onCatStateChange += ProcessAction_OnCatState;
    }

    private void OnDisable()
    {
        
    }

    #region EventMethods

    void ProcessAction_OnCatLocation(CatLocation location)
    {
        
    }

    void ProcessAction_OnCatState(CatState state)
    {
        
    }
    

    #endregion
}
