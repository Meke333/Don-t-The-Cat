using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    #region Events

    public Action onPlayerDied;
    public Action onWonGame;
    public Action<double> onCatInteraction;
    public Action<CatState> onCatReaction;


    #endregion

    private void Awake()
    {
        Instance = this;
    }
    
    
}
