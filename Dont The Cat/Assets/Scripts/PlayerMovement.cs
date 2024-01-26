using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerState state;

    public float playerSpeed;
    
    public float _xInput;
    public float _yInput;

    public Vector3 directionForward;
    public Vector3 directionRight;

    public Vector2 mouseSensitivity;
    public float clampValuesXRotation;
    
    public float _mouseX;
    public float _mouseY;
    private float xRotation, yRotation;

    public CharacterController CharacterController;

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Looking();
        Walking();
    }

    void GetInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");

    }
    
    void Walking()
    {
        if (state != PlayerState.Walking)
            return;

        //Vector3 currentPosition = gameObject.transform.position;

        directionForward = gameObject.transform.forward;
        directionForward.y = 0;

        directionRight = gameObject.transform.right;
        directionRight.y = 0;

        //directionForward = direction;

        //transform.position += Time.deltaTime * playerSpeed * direction;

        CharacterController.Move(Time.deltaTime * playerSpeed * ((_yInput * directionForward) + (_xInput * directionRight)));
    }

    void Looking()
    {
        xRotation -= _mouseY * Time.deltaTime * mouseSensitivity.y;
        yRotation += _mouseX * Time.deltaTime * mouseSensitivity.x;
        
        xRotation = Mathf.Clamp(xRotation, -clampValuesXRotation, clampValuesXRotation);
        
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        //transform.Rotate(_mouseY * Time.deltaTime * Vector3.right);
        //transform.Rotate( _mouseX * Time.deltaTime * Vector3.up);
    }
    
    
}

public enum PlayerState
{
    Walking,
    Petting,
    Working,
}
