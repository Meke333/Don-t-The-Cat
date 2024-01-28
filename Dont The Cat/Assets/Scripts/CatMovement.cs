using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CatMovement : MonoBehaviour
{
    private CatScript _catScript;

    public int jumpCounter = 0;

    private bool _canJump;

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

    public LayerMask layerMask;

    public float angle;
    public Vector3 cross;
    public float distanceToTarget;
    
    [Space]
    [Header("GAME DESIGN")]
    
    public float speed;
    public float gravity = 20;
    public float jumpSpeed = 8;
    

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
        Vector3 pos = transform.position;
        
        if (pos.y < 0.283f)
        {
            pos.y = 0.283f;
            transform.position = pos;
        }
        
        if (!_isTargetActive)
            return;

        //Vector3 nextPosition = (Time.deltaTime * speed * transform.forward) + transform.position;

        if (characterController.isGrounded)
        {
            //distanceToTarget = Mathf.Max((targetPosition - transform.position).magnitude, 1);

            moveDirection = speed * transform.forward;
            
            if (_canJump)
            {
                moveDirection.y = jumpSpeed;
                _canJump = false;
            }
        }
        
        
        moveDirection.y -= gravity * Time.deltaTime;
        

        //rigidbody.MovePosition( nextPosition);
        characterController.Move(Time.deltaTime * moveDirection);

        /*
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        angle = Vector3.Angle(currentDirection, targetDirection) ;
        cross = Vector3.Cross(currentDirection, targetDirection);
        if (cross.y < 0) angle = -angle;
        
        
        gameObject.transform.Rotate(0,angle * Time.deltaTime, 0);
        */
        
        
        targetPosition.y = transform.position.y;
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
        _canJump = true;
        jumpCounter++;
        

        await Task.Delay(1000);
        _canJump = false;
        _isTargetActive = false;

        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.y += 180;
        
        gameObject.transform.rotation = Quaternion.Euler(rotation);
        
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
        _isTargetActive = true;

        targetPosition.y = transform.position.y;
        gameObject.transform.LookAt(targetPosition);
        
        /*Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        angle = Vector3.Angle(currentDirection, targetDirection) ;
        cross = Vector3.Cross(currentDirection, targetDirection);
        if (cross.y < 0) angle = -angle;
        

        gameObject.transform.Rotate(0,angle, 0);
        */
    }

    void ProcessAction_OnCatReaction(CatState state)
    {
        if (state == CatState.Pleased)
            jumpCounter = 0;
    }

    #endregion
}
