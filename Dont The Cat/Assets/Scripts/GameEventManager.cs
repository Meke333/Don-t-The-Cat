using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public GameEventManager Instance;

    #region Events

    public Action onPlayerDied;
    public Action onWonGame;
    public Action<double> onCatInteraction;


    #endregion

    private void Awake()
    {
        Instance = this;
    }
    
    
}
