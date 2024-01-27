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

    private void Awake()
    {
        _catScript = GetComponent<CatScript>();
    }

    private async void OnEnable()
    {
        await Task.Yield();
        GameEventManager.Instance.onCatLocationSet += ProcessAction_OnCatLocationSet;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.onCatLocationSet -= ProcessAction_OnCatLocationSet;
    }

    private void Update()
    {
        if (!_isTargetActive)
            return;

        characterController.Move( Time.deltaTime * speed * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
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
        
        _isTargetActive = false;
        
        _catScript.onCatJump?.Invoke();
        
        //Activate Jump Sequence
        JumpSequence();
    }

    #region Methods

    async void JumpSequence()
    {
        float currentTime = 0;

        while (currentTime < jumpDurationInSeconds)
        {
            float progressPercentage = currentTime/jumpDurationInSeconds;

            characterController.Move((Vector3.up * progressPercentage) + (Time.deltaTime * jumpDistanceForward * Vector3.forward));
            
            currentTime += Time.deltaTime;
            await Task.Yield();
        }
        
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
        
        gameObject.transform.LookAt(targetPosition);
        _isTargetActive = true;

    }

    #endregion
}
