using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatMovement : MonoBehaviour
{
    private CatScript _catScript;

    public AnimationCurve jumpPositionAnimation;
    public float jumpDistanceForward;
    public float jumpDurationInSeconds;
    
    public float speed;
    public float gravity = 20;
    public float jumpSpeed = 8;
    public int jumpCounter = 0;

    private bool jumppls;

    public Vector3 moveDirection = Vector3.zero;
    
    [SerializeField] private CatLocation targetLocationEnum;
    public Vector3 targetPosition;
    [SerializeField] private bool _isTargetActive;

    public Transform IdlePosition;
    public Transform VasePosition;
    public Transform UrnePosition;
    public Transform RadioPosition;
    public Transform SelfDestructButtonPosition;

    public CharacterController characterController;
    public Rigidbody rigidbody;

    public LayerMask layerMask;

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
    }

    private async void OnEnable()
    {
        await Task.Yield();
        GameEventManager.Instance.onCatLocationSet += ProcessAction_OnCatLocationSet;
        GameEventManager.Instance.onCatReaction += ProcessAction_OnCatReaction;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.onCatLocationSet -= ProcessAction_OnCatLocationSet;
        GameEventManager.Instance.onCatReaction -= ProcessAction_OnCatReaction;
    }

    private void Update()
    {
        if (!_isTargetActive)
            return;

        //Vector3 nextPosition = (Time.deltaTime * speed * transform.forward) + transform.position;

        if (characterController.isGrounded)
        {
            moveDirection = transform.forward * speed;
            
            if (jumppls)
            {
                moveDirection.y = jumpSpeed;
                jumppls = false;
            }
        }
        
        
        moveDirection.y -= gravity * Time.deltaTime;
        

        //rigidbody.MovePosition( nextPosition);
        characterController.Move(Time.deltaTime * moveDirection);

        gameObject.transform.LookAt(targetPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (jumpCounter > 0)
            return;
        
        switch (other.gameObject.tag)
        {
            case "cat_location_vase":
                if (targetLocationEnum != CatLocation.Vase)
                    return;
                break;
            case "cat_location_urne":
                if (targetLocationEnum != CatLocation.Urne)
                    return;
                break;
            case "cat_location_radio":
                if (targetLocationEnum != CatLocation.Radio)
                    return;
                break;
            case "cat_location_selfdestruct_button":
                if (targetLocationEnum != CatLocation.SelfDestructButton)
                    return;
                break;
        }

        _catScript.onCatJump?.Invoke();
        
        //Activate Jump Sequence
        JumpSequence();
    }

    #region Methods

    async void JumpSequence()
    {
        Debug.Log("JUMP");
        jumppls = true;
        jumpCounter++;
        

        await Task.Delay(2000);
        jumppls = false;
        _isTargetActive = false;
        
        _catScript.onCatLanded.Invoke();
        GameEventManager.Instance.onCatNearTheObject?.Invoke();
    }

    #endregion

    #region EventMethods

    void ProcessAction_OnCatLocationSet(CatLocation target)
    {
        targetLocationEnum = target;

        switch (target)
        {
            case CatLocation.Nothing:
                targetPosition = IdlePosition.position;
                break;
            case CatLocation.Vase:
                targetPosition = VasePosition.position;
                break;
            case CatLocation.Urne:
                targetPosition = UrnePosition.position;
                break;
            case CatLocation.Radio:
                targetPosition = RadioPosition.position;
                break;
            case CatLocation.SelfDestructButton:
                targetPosition = SelfDestructButtonPosition.position;
                break; 
                
        }
        
        targetPosition.y = 0;
        gameObject.transform.LookAt(targetPosition);
        _isTargetActive = true;

    }

    void ProcessAction_OnCatReaction(CatState state)
    {
        if (state == CatState.Pleased)
            jumpCounter = 0;
    }

    #endregion
}
