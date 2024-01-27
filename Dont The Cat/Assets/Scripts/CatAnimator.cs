using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimator : MonoBehaviour
{
    public Animator catAnimator;

    struct AnimatorVariables
    {
        private static readonly int Location = Animator.StringToHash("Location");
        private static readonly int IntoLocationMovement = Animator.StringToHash("IntoLocationMovement");
        private static readonly int EnterToAnticipation = Animator.StringToHash("EnterToAnticipation");
        private static readonly int AnticipationToTheIncident = Animator.StringToHash("AnticipationToTheIncident");
        private static readonly int IsPetting = Animator.StringToHash("IsPetting");
        private static readonly int InPetState = Animator.StringToHash("InPetState");
        
    }
}
