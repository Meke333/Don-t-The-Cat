using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private PlayerState _playerState;
    
    public GameObject PetCursorInactive;
    public GameObject PetCursorActive;
    public GameObject ButtonCursor;
    public GameObject NothingCursor;

    public GameObject ActiveCursor;

    private Vector3 mousePosition;
    public Vector3 mouseOffset;

    private void Awake()
    {
        ActiveCursor = NothingCursor;
        ActiveCursor.SetActive(true);
    }

    private async void OnEnable()
    {
        await Task.Yield();
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
            ActiveCursor.SetActive(false);
            ActiveCursor = PetCursorActive;
            ActiveCursor.SetActive(true);
        }
        else
        {
            
            ActiveCursor.SetActive(false);
            ActiveCursor = PetCursorInactive;
            ActiveCursor.SetActive(true);
        }
    }

    void ProcessAction_onPlayerStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Walking:
                
                ActiveCursor.SetActive(false);
                ActiveCursor = NothingCursor;
                break;
            case PlayerState.Petting:
                
                ActiveCursor.SetActive(false);
                ActiveCursor = PetCursorInactive;
                ActiveCursor.SetActive(true);
                break;
            case PlayerState.Working:
                
                ActiveCursor.SetActive(false);
                ActiveCursor = ButtonCursor;
                ActiveCursor.SetActive(true);
                break;
            case PlayerState.LookAtDisaster:
                
                ActiveCursor.SetActive(false);
                ActiveCursor = NothingCursor;
                break;
            case PlayerState.Die:
                
                ActiveCursor.SetActive(false);
                ActiveCursor = NothingCursor;
                break;
        }
    }

    #endregion
}
