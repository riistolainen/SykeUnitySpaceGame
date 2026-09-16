using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    public bool debug = false;

    GameObject spaceshipRef;
    SpaceShipScript mySpaceshipScript;

    GameObject camerasRef;
    CamerasScript myCamerasScript;
/* Option for disabling some actions by disabling input components. Currently working the components themselves by just reacting to input. Thus - InputManager monitors and parses input from user to the system. Systems choose how they react to it.
    InputActionMap uiActionMap;
    InputActionMap pilotActionMap;
    InputActionMap cameraActionMap;
*/
    private InputAction thrust, roll, yaw, pitch;
    private float thrustValue, rollValue, yawValue, pitchValue;

    private InputAction cameraZoom;
    private float zoomValue;

    private InputAction lookAroundState;
    private InputAction pilotState;

    public enum StateControl
    {
        UI = 1,
        Pilot = 2,
        Camera = 3
    };

    private StateControl _currentState = StateControl.UI; //private value only for internal use
    public StateControl CurrentState    //Class
    {
        get { return _currentState; }
        set
        {
            if (_currentState == value) { return; }
            else
            {
                Debug.Log("InputManager: ControlStateChange before listener: " + value + " -> " + _currentState);
                _currentState = value;

                if (OnControlStateChange != null)   //Someone listening on Event
                {
                    Debug.Log("InputManager: Invoke OnControlStateChange " + _currentState);
                    OnControlStateChange(_currentState);
                }
                else { Debug.Log("InputManager: No Listeners"); }
            }
        }
    }

    //public delegate void OnControlStateChange(stateControl newState);
    public event Action<StateControl> OnControlStateChange;
    

    private void UpdateState()  //Only control what is requested
    {
        if (lookAroundState.IsPressed() && _currentState != StateControl.Camera) // lookAround is held down and mode is not active
        {
            CurrentState = StateControl.Camera;
            //myCamerasScript.lookAroundToggle = true;
            
            //uiActionMap.Disable(); pilotActionMap.Disable(); cameraActionMap.Enable();
            Debug.Log("State: Looking");
        }

        if (pilotState.IsPressed() && _currentState != StateControl.Pilot) // pilot is held down and mode is not active
        {
            CurrentState = StateControl.Pilot;
            //myCamerasScript.pilotToggle = true;
            //uiActionMap.Disable(); pilotActionMap.Enable(); cameraActionMap.Disable();
            Debug.Log("State: Piloting");
        }

        if (!lookAroundState.IsPressed() && !pilotState.IsPressed() && _currentState != StateControl.UI)  //No active control modifiers
        {
            CurrentState = StateControl.UI; //Default
            //myCamerasScript.lookAroundToggle = false;
            //myCamerasScript.pilotToggle = false;
            //uiActionMap.Enable(); pilotActionMap.Disable(); cameraActionMap.Disable();
            Debug.Log("State: UI");
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = StateControl.UI;
        
        //TODO: defineActionMaps
        pilotState = InputSystem.actions.FindAction("PilotState");              //Left-SHIFT
        lookAroundState = InputSystem.actions.FindAction("LookAroundState");    //Left-CTRL

        //UI

        //CAMERA
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");              //Mouse scrollwheel

        //PILOT
        thrust = InputSystem.actions.FindAction("MainThruster");
        roll = InputSystem.actions.FindAction("Roll");
        yaw = InputSystem.actions.FindAction("Yaw");
        pitch = InputSystem.actions.FindAction("Pitch");


        //Find links to other objects that the inputs relate to
        
        //SPACESHIP
        spaceshipRef = GameObject.Find("SpaceShip");
        if (spaceshipRef == null)
        {
            if (debug) { Debug.LogWarning("InputManager: Player object not found."); }
        }
        else
        {
            mySpaceshipScript = spaceshipRef.GetComponent<SpaceShipScript>();
            if (mySpaceshipScript == null)
            {
                if (debug) { Debug.LogWarning("InputManager: GameManagerScript not found."); }
            }
            
        }

        //CAMERAS
        camerasRef = GameObject.Find("Cameras");
        if (camerasRef == null)
        {
            if (debug) { Debug.LogWarning("InputManager: Cameras object not found."); }
        }
        else
        {
            myCamerasScript = camerasRef.GetComponent<CamerasScript>();
            if (myCamerasScript == null)
            {
                if (debug) { Debug.LogWarning("InputManager: CamerasScript not found."); }
            }

        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateState();  //Based on input update state that modifies how to the inputs are interpreted

        //TODO: Update() or FixedUpdate()? more performant, but less reactive control?

        if (_currentState == StateControl.Camera)
        {
            if (cameraZoom.WasPressedThisFrame())
            {
                zoomValue = cameraZoom.ReadValue<float>() * Time.unscaledDeltaTime;
                myCamerasScript.ZoomCamera(zoomValue);
                zoomValue = 0f;
            }
        }

        if (_currentState == StateControl.Pilot)  //Only control ship when cursor is locked - when unlocked user is engaged with UI
        {
            if (thrust.IsPressed())
            {
                mySpaceshipScript.thrustActive = true;

                thrustValue += thrust.ReadValue<float>() * Time.deltaTime;
            }

            if (roll.IsPressed())
            {
                mySpaceshipScript.rollActive = true;

                rollValue -= roll.ReadValue<float>() * Time.deltaTime;
            }

            if (yaw.IsPressed())
            {
                mySpaceshipScript.yawActive = true;

                yawValue += yaw.ReadValue<float>() * Time.deltaTime;
            }

            if (pitch.IsPressed())
            {
                mySpaceshipScript.pitchActive = true;

                pitchValue += pitch.ReadValue<float>() * Time.deltaTime;
            }
            mySpaceshipScript.Pilot(thrustValue, rollValue, yawValue, pitchValue);
            thrustValue = 0; rollValue = 0; yawValue = 0; pitchValue = 0;   //reset applied values
        }
    }
}