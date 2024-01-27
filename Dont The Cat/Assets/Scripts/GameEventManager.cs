using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    #region Events

    public Action onPlayerDied;
    public Action onWonGame;
    public Action<CatLocation> onCatLocationSet;
    public Action<double> onCatInteraction;
    public Action<CatState> onCatReaction;
    public Action onCatInPetState;
    public Action onCatGettingPet;
    public Action onCatNearTheObject;


    #endregion

    private void Awake()
    {
        Instance = this;
    }
    
    
}
