using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBehavior : MonoBehaviour
{

    public CatState state;
    private void Update()
    {
        ProcessState();
    }

    #region Methods

    void ProcessState()
    {
        switch (state)
        {
            case CatState.Nothing:
                Nothing_State();
                break;
        }
    }

    void Nothing_State()
    {
        
    }

    #endregion
}

public enum CatState
{
    Nothing,
    
}
