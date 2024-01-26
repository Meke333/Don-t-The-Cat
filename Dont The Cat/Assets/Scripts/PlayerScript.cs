using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private PlayerState _state;
    
    public Action<PlayerState> onPlayerStateChange;
    
    
}

public enum PlayerState
{
    Walking,
    Petting,
    Working,
}