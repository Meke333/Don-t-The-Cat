using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour
{
    private CatLocation _location;
    private CatState _state;

    public Action<CatLocation> onCatLocationChange;
    public Action<CatState> onCatStateChange;
    public Action onCatJump;
    public Action onCatLanded;

    private void OnEnable()
    {
        onCatLocationChange += value => _location = value;
        onCatStateChange += value => _state = value;
    }
}

public enum CatLocation
{
    Nothing,
    Vase,
    Urne,
    Radio,
    SelfDestructButton
}

public enum CatState
{
    Unpetted,
    InPetMode,
    Pleased,
    Overpetted,
    UnderPetted,
}