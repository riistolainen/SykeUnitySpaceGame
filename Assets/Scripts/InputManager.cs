using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool debug = false;

    GameObject spaceshipRef;
    SpaceShipScript mySpaceshipScript;

    InputActionMap UIActionMap;
    InputActionMap pilotActionMap;
    InputActionMap cameraActionMap;

    private InputAction lookAroundState;
    private InputAction pilotState;

    private InputAction thrust, roll, yaw, pitch;

    private InputAction cameraZoom;

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
            UIActionMap.Disable(); pilotActionMap.Disable(); cameraActionMap.Enable();
        }

        if (pilotState.IsPressed() && (int)currentState != 2) // pilot is held down and mode is not active
        {
            currentState = stateControl.Pilot;
            UIActionMap.Disable(); pilotActionMap.Enable(); cameraActionMap.Disable();
        }

        if (!lookAroundState.IsPressed() && !pilotState.IsPressed() && (int)currentState != 1)  //No active control modifiers
        {
            currentState = stateControl.UI; //Default
            UIActionMap.Enable(); pilotActionMap.Disable(); cameraActionMap.Disable();
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
        spaceshipRef = GameObject.Find("Player");
        if (spaceshipRef == null)
        {
            if (debug) { Debug.LogWarning("Player object not found."); }
        }
        else
        {
            mySpaceshipScript = spaceshipRef.GetComponent<SpaceShipScript>();
            if (mySpaceshipScript == null)
            {
                if (debug) { Debug.LogWarning("GameManagerScript not found."); }
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
                zoom = -cameraZoom.ReadValue<float>();
                //TODO: Call cameras to zoom
                /*
                cameraZoomed = true;
                Debug.Log("cameraZoomed:true " + zoom);
                */
            }
        }

        if (currentState == stateControl.Pilot)  //Only control ship when cursor is locked - when unlocked user is engaged with UI
        {//TODO: Fix camera when piloting; separate button to enable certain thrusters? CTRL/SHIFT/etc. or something else?
            if (thrust.IsPressed())
            {
                mySpaceshipScript.thrustActive = true;
            }

            if (roll.IsPressed())
            {
                mySpaceshipScript.rollActive = true;
            }

            if (yaw.IsPressed())
            {
                mySpaceshipScript.yawActive = true;
            }

            if (pitch.IsPressed())
            {
                mySpaceshipScript.pitchActive = true;
            }
        }
    }
}
