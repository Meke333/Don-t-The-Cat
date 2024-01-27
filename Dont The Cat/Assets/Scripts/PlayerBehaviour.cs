using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBehaviour : MonoBehaviour
{
    private PlayerScript _playerScript;
    
    public PlayerState state;

    public float playerSpeed;
    
    public float _xInput, _yInput; //vertical and horizontal movement tracker [-1, 0, 1]

    public Vector3 directionForward, directionRight; //direction vector for foward and sideward

    public Vector2 mouseSensitivity; //sensitivity for rotation movement
    public float clampValuesXRotation; //min/max value for the x-rotation (up and down)
    
    public float _mouseX, _mouseY; //mouse position
    public float _lastMouseX, _lastMouseY; //last mouse position
    private float xRotation, yRotation; //rotation of the player based on the mouse

    private double catMeter;

    public CharacterController CharacterController; //for moving around
    public Transform CameraPosition; //for looking around

    public Animator cameraAnimator; //for idle and walk animation (going down and up)

    public Material cube1Material; //for DEBUG purposes
    public Material cube2Material; //for DEBUG purposes

    public TextMeshProUGUI triggerText; //for entering and leaving cat-/work-mode

    private bool inCatLocation = false;
    private bool inWorkLocation = false;

    private void Awake()
    {
        _playerScript = GetComponent<PlayerScript>();
        triggerText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _playerScript.onPlayerStateChange += ProcessAction_OnPlayerStateChange;
    }

    private void OnDisable()
    {
        _playerScript.onPlayerStateChange -= ProcessAction_OnPlayerStateChange;
    }

    private void setTriggerText(bool visible, String content)
    {
        triggerText.gameObject.SetActive(visible);
        triggerText.text = content;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "cat_location_1":
                setTriggerText(true, "press Q");
                inCatLocation = true;
                inWorkLocation = false;
                break;
            case "work_location":
                setTriggerText(true, "press Q");
                inWorkLocation = true;
                inCatLocation = false;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "cat_location_1":
                setTriggerText(false, "");
                inCatLocation = false;
                break;
            case "work_location":
                setTriggerText(false, "");
                inWorkLocation = false;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Walking();
        Working();
        Petting();

        if(inCatLocation && Input.GetKeyDown(KeyCode.Q)) //in triggerbox and pressed q -> go into petting mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Petting);
            setTriggerText(true, "press E");
        } else if(inWorkLocation && Input.GetKeyDown(KeyCode.Q)) //in triggerbox and pressed q -> go into working mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Working);
            setTriggerText(true, "press E");
        } else if((inCatLocation || inWorkLocation) && Input.GetKeyDown(KeyCode.E)) //in triggerbox and pressed e -> go into walking mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Walking);
            setTriggerText(true, "press Q");
        }
    }

    void GetInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        _lastMouseX = _mouseX;
        _lastMouseY = _mouseY;
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");

    }
    
    void Walking()
    {
        if (state != PlayerState.Walking)
            return;

        Looking();

        cube1Material.color = new Color(0, 0, 1);
        cube2Material.color = new Color(0, 0, 1);

        //forward direction of camera
        directionForward = CameraPosition.forward;
        directionForward.y = 0;

        //sideward direction of camera
        directionRight = CameraPosition.right;
        directionRight.y = 0;

        //Move Player in direction of sight
        CharacterController.Move(Time.deltaTime * playerSpeed * ((_yInput * directionForward) + (_xInput * directionRight)));

        //Camera up and down movement
        if (_xInput != 0 || _yInput != 0)
            cameraAnimator.SetBool("IsWalking", true);
        else
            cameraAnimator.SetBool("IsWalking", false);
    }

    void Working()
    {
        if (state != PlayerState.Working)
            return;

        cube2Material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        cube1Material.color = new Color(0, 0, 1);

    }
    void Petting()
    {
        if (state != PlayerState.Petting)
            return;

        Looking();

        cube1Material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        cube2Material.color = new Color(0, 0, 1);

        // �berpr�ft, ob die linke Maustaste gedr�ckt wird
        if (Input.GetMouseButton(0))
        {
            // Wandelt die Mausposition in einen Ray um
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Struktur, um Informationen �ber den Raycast zu speichern
            RaycastHit hit;

            // F�hrt den Raycast aus
            if (Physics.Raycast(ray, out hit))
            {
                // Hit-Informationen nutzen
                GameObject targetObject = hit.collider.gameObject;
                Debug.Log("Detected object: " + targetObject.name);
                if (targetObject.CompareTag("cat"))
                {
                    float mouse_x_dif = _mouseX - _lastMouseX;
                    float mouse_y_dif = _mouseY - _lastMouseY;
                    double distance = Math.Sqrt(Math.Pow((double)mouse_x_dif, 2) + Math.Pow((double)mouse_y_dif, 2));

                    catMeter = (catMeter < 1.0 ? catMeter + distance * 1.4 * Time.deltaTime : 1.0);
                }
            }
        }
        else
        {
            catMeter = (catMeter > 0.0 ? catMeter - 0.01 * Time.deltaTime: 0.0);
        }

        Debug.Log("Cat-Meter: " + catMeter);
    }

    void Looking()
    {
        xRotation -= _mouseY * Time.deltaTime * mouseSensitivity.y;
        yRotation += _mouseX * Time.deltaTime * mouseSensitivity.x;
        
        xRotation = Mathf.Clamp(xRotation, -clampValuesXRotation, clampValuesXRotation);

        CameraPosition.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        //transform.Rotate(_mouseY * Time.deltaTime * Vector3.right);
        //transform.Rotate( _mouseX * Time.deltaTime * Vector3.up);
    }

    #region EventMethods

    void ProcessAction_OnPlayerStateChange(PlayerState newState)
    {
        state = newState;
    }

    #endregion
    
    
}

