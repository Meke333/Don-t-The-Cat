using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    private PlayerScript _playerScript;
    
    public PlayerState state;

    public float playerSpeed;
    
    public float horizontalInput, verticalInput; //vertical and horizontal movement tracker [-1, 0, 1]

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

    //public TextMeshProUGUI triggerText; //for entering and leaving cat-/work-mode
    public Image triggerIcon;
    public Sprite q_key_icon, e_key_icon, mousewiggle_key_icon;

    public Transform CatPosition;

    private bool inCatLocation = false;
    private bool inWorkLocation = false;

    public CatLocation cat_location;

    private Timer catPettingTimer = new Timer();
    public int catPettingTime;
    bool _isCatPetTimerActive;

    public bool isCatPetAble;

    private void Awake()
    {
        _playerScript = GetComponent<PlayerScript>();
        triggerIcon.gameObject.SetActive(false);
        catPettingTimer.SetTimer(catPettingTime);
    }

    async private void OnEnable()
    {
        _playerScript.onPlayerStateChange += ProcessAction_OnPlayerStateChange;
        catPettingTimer.onTimerDone += ProcessAction_CatTimerDone;
        await Task.Yield();
        GameEventManager.Instance.onCatLocationSet += ProcessAction_OnCatLocationSet;
        GameEventManager.Instance.onCatNearTheObject += ProcessAction_OnCatNearTheObject;
        GameEventManager.Instance.onCatReaction += ProcessAction_onCatReaction;
    }

    private void OnDisable()
    {
        _playerScript.onPlayerStateChange -= ProcessAction_OnPlayerStateChange;
        catPettingTimer.onTimerDone -= ProcessAction_CatTimerDone;
        GameEventManager.Instance.onCatLocationSet -= ProcessAction_OnCatLocationSet;
        GameEventManager.Instance.onCatNearTheObject -= ProcessAction_OnCatNearTheObject;
        GameEventManager.Instance.onCatReaction -= ProcessAction_onCatReaction;
    }

    private void setTriggerKeyIcon(bool visible, int key) //key -> 0:Q, 1:E
    {
        triggerIcon.gameObject.SetActive(visible);
        triggerIcon.sprite = (key == 0 ? q_key_icon : (key == 1 ? e_key_icon : mousewiggle_key_icon));
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isCatPetAble)
            return;
        
        switch (other.gameObject.tag)
        {
            case "cat_location_vase":
                if (cat_location == CatLocation.Vase)
                {
                    setTriggerKeyIcon(true, 0); //show Q
                    inCatLocation = true;
                    inWorkLocation = false;
                }
                break;
            case "cat_location_urne":
                if (cat_location == CatLocation.Urne)
                {
                    setTriggerKeyIcon(true, 0); //show Q
                    inCatLocation = true;
                    inWorkLocation = false;
                }
                break;
            case "cat_location_radio":
                if (cat_location == CatLocation.Radio)
                {
                    setTriggerKeyIcon(true, 0); //show Q
                    inCatLocation = true;
                    inWorkLocation = false;
                }
                break;
            case "cat_location_selfdestruct_button":
                if (cat_location == CatLocation.SelfDestructButton)
                {
                    setTriggerKeyIcon(true, 0); //show Q
                    inCatLocation = true;
                    inWorkLocation = false;
                }
                break;
            case "work_location":
                setTriggerKeyIcon(true, 0); //show Q
                inWorkLocation = true;
                inCatLocation = false;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "cat_location_vase":
            case "cat_location_urne":
            case "cat_location_radio":
            case "cat_location_selfdestruct_button":
                setTriggerKeyIcon(false, 0); //hide
                inCatLocation = false;
                break;
            case "work_location":
                setTriggerKeyIcon(false, 0); //hide
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
        LookAtDisaster();
        Die();


        if (state != PlayerState.Petting && inCatLocation && Input.GetKeyDown(KeyCode.Q)) //in pet-triggerbox and pressed q -> go into petting mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Petting);
            setTriggerKeyIcon(true, 2); //show Mousewiggle

            //Look at cat
            CameraPosition.LookAt(CatPosition); 

            if (!_isCatPetTimerActive)
            { 
                catPettingTimer.RunTimer();
                _isCatPetTimerActive = true;
            }
        } else if(state != PlayerState.Working && inWorkLocation && Input.GetKeyDown(KeyCode.Q)) //in work-triggerbox and pressed q -> go into working mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Working);
            setTriggerKeyIcon(true, 1); //show E
        } else if(inWorkLocation && Input.GetKeyDown(KeyCode.E)) //in work-triggerbox and pressed e -> go into walking mode
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Walking);
            setTriggerKeyIcon(true, 0); //show Q
        }
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

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
        CharacterController.Move(Time.deltaTime * playerSpeed * ((verticalInput * directionForward) + (horizontalInput * directionRight)));

        //Camera up and down movement
        if (horizontalInput != 0 || verticalInput != 0)
            cameraAnimator.SetBool("IsWalking", true);
        else
            cameraAnimator.SetBool("IsWalking", false);
    }

    void Working()
    {
        if (state != PlayerState.Working)
            return;
        
        //Stop walking
        cameraAnimator.SetBool("IsWalking", false);

        cube2Material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        cube1Material.color = new Color(0, 0, 1);

    }
    void Petting()
    {
        if (state != PlayerState.Petting)
            return;
        
        //Stop walking
        cameraAnimator.SetBool("IsWalking", false);

        cube1Material.color = new Color(1 - (float)catMeter, (float)catMeter, 0);
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
                //Debug.Log("Detected object: " + targetObject.name);
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

        //Debug.Log("Cat-Meter: " + catMeter);
    }

    void LookAtDisaster()
    {
        if (state != PlayerState.LookAtDisaster)
            return;
        
        //Stop walking
        cameraAnimator.SetBool("IsWalking", false);

        //Move backwards a bit

        //look at disaster -> CameraPosition.LookAt(object);
    }

    void Die()
    {
        if (state != PlayerState.Die)
            return;

        //Stop walking
        cameraAnimator.SetBool("IsWalking", false);

        //Look at cat
        CameraPosition.LookAt(CatPosition);

        //SceneManager.LoadScene("MENU");
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

        //reset catMeter when entering a mode
        catMeter = 0;
    }

    void ProcessAction_CatTimerDone()
    {
        isCatPetAble = false;
        _isCatPetTimerActive = false;
        GameEventManager.Instance.onCatInteraction?.Invoke(catMeter);
    }

    void ProcessAction_onCatReaction(CatState state)
    {
        if(state == CatState.Pleased)
        {
            _playerScript.onPlayerStateChange.Invoke(PlayerState.Walking);
            setTriggerKeyIcon(false, 0); //hide
        }
    }

    void ProcessAction_OnCatLocationSet(CatLocation location)
    {
        cat_location = location;
    }

    void ProcessAction_OnCatNearTheObject()
    {
        isCatPetAble = true;
    }

    #endregion


}


