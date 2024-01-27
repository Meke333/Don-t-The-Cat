using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
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
        Debug.Log("Hello: " + other.gameObject.layer + layerMask.value);
        
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
            return;

        _isTargetActive = false;
        
        //Activate Jump Sequence
        /////////////////////////
    }

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
