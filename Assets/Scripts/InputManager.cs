using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool debug = false;

    GameObject spaceshipRef;
    SpaceShipScript mySpaceshipScript;

    GameObject camerasRef;
    CamerasScript myCamerasScript;

    InputActionMap uiActionMap;
    InputActionMap pilotActionMap;
    InputActionMap cameraActionMap;

    private InputAction lookAroundState;
    private InputAction pilotState;

    private InputAction thrust, roll, yaw, pitch;
    private float thrustValue, rollValue, yawValue, pitchValue;

    private InputAction cameraZoom;
    private float zoomValue;

    public enum stateControl
    {
        UI = 1,
        Pilot = 2,
        Camera = 3
    };

    public stateControl currentState = stateControl.UI;

    private void UpdateState()  //Only control what is requested
    {
        if (lookAroundState.IsPressed() && (int)currentState != 3) // lookAround is held down and mode is not active
        {
            currentState = stateControl.Camera;
            myCamerasScript.lookAroundToggle = true;
            //uiActionMap.Disable(); pilotActionMap.Disable(); cameraActionMap.Enable();
            Debug.Log("State: Looking");
        }

        if (pilotState.IsPressed() && (int)currentState != 2) // pilot is held down and mode is not active
        {
            currentState = stateControl.Pilot;
            myCamerasScript.pilotToggle = true;
            //uiActionMap.Disable(); pilotActionMap.Enable(); cameraActionMap.Disable();
            Debug.Log("State: Piloting");
        }

        if (!lookAroundState.IsPressed() && !pilotState.IsPressed() && (int)currentState != 1)  //No active control modifiers
        {
            currentState = stateControl.UI; //Default
            myCamerasScript.lookAroundToggle = false;
            myCamerasScript.pilotToggle = false;
            //uiActionMap.Enable(); pilotActionMap.Disable(); cameraActionMap.Disable();
            Debug.Log("State: UI");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    void Update()
    {
        UpdateState();  //Based on input update state that modifies how to the inputs are interpreted

        if (currentState == stateControl.Camera)
        {
            if (cameraZoom.WasPressedThisFrame())
            {
                zoomValue = cameraZoom.ReadValue<float>();
                myCamerasScript.ZoomCamera(zoomValue);
                zoomValue = 0f;
            }
        }

        if (currentState == stateControl.Pilot)  //Only control ship when cursor is locked - when unlocked user is engaged with UI
        {//TODO: Fix camera when piloting; separate button to enable certain thrusters? CTRL/SHIFT/etc. or something else?
            if (thrust.IsPressed())
            {
                mySpaceshipScript.thrustActive = true;

                thrustValue += thrust.ReadValue<float>();
            }

            if (roll.IsPressed())
            {
                mySpaceshipScript.rollActive = true;

                rollValue -= roll.ReadValue<float>();
            }

            if (yaw.IsPressed())
            {
                mySpaceshipScript.yawActive = true;

                yawValue += yaw.ReadValue<float>();
            }

            if (pitch.IsPressed())
            {
                mySpaceshipScript.pitchActive = true;

                pitchValue += pitch.ReadValue<float>();
            }
            mySpaceshipScript.Pilot(thrustValue, rollValue, yawValue, pitchValue);
            thrustValue = 0; rollValue = 0; yawValue = 0; pitchValue = 0;   //reset applied values
        }
    }
}