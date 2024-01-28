using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private PlayerState _playerState;
    
    public GameObject PetCursorInactive;
    public GameObject PetCursorActive;

    public GameObject ButtonCursor;

    public GameObject ActiveCursor;

    private Vector3 mousePosition;
    public Vector3 mouseOffset;

    private void Awake()
    {
        ActiveCursor = PetCursorActive;
        ActiveCursor.SetActive(true);
    }

    private void OnEnable()
    {
        GameEventManager.Instance.onPlayerStateChange += ProcessAction_onPlayerStateChange;
        GameEventManager.Instance.onCatGettingPet += ProcessAction_onCatGettingPet;
    }

    private void Update()
    {
        
        
        mousePosition = Input.mousePosition;

        ActiveCursor.transform.position = mousePosition + mouseOffset;



    }

    #region EventMethode

    void ProcessAction_onCatGettingPet(bool isPet)
    {
        if (isPet)
        {
            ActiveCursor = PetCursorActive;
        }
        else
        {
            ActiveCursor = PetCursorInactive;
        }
    }

    void ProcessAction_onPlayerStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Walking:
                ActiveCursor = null;
                break;
            case PlayerState.Petting:
                ActiveCursor = PetCursorInactive;
                break;
            case PlayerState.Working:
                ActiveCursor = ButtonCursor;
                break;
            case PlayerState.LookAtDisaster:
                ActiveCursor = null;
                break;
            case PlayerState.Die:
                ActiveCursor = null;
                break;
        }
    }

    #endregion
}
