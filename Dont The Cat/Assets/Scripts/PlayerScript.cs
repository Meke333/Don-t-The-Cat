using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private PlayerState _state;
    
    public Action<PlayerState> onPlayerStateChange;
    
    private void OnEnable()
    {
        onPlayerStateChange += value => _state = value;
    }

    private void OnDisable()
    {
        onPlayerStateChange -= value => _state = value;
    }
}

public enum PlayerState
{
    Walking,
    Petting,
    Working,
}