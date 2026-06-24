using Unity.Cinemachine;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class CamerasScriptable : MonoBehaviour
{
    [CreateProperty] public CinemachineCamera CameraFollow;
    [CreateProperty] public CinemachineCamera CameraOverhead;
    [CreateProperty] public CinemachineCamera CameraFreefly;

    public InputAction cameraZoom;
    public InputAction cameraLock;
    private bool cameraToggle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Controls
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");  //mouse scrollwheel
        cameraLock = InputSystem.actions.FindAction("CameraLock");  //TAB

        //GET REF TO CAMERAS.obj
        GameObject camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }

        CameraFollow = gameObject.transform.Find("CinemachineCameraFollow").GetComponent<CinemachineCamera>();
        if (!CameraFollow) { Debug.LogError(this.name + "/" +CameraFollow.name + ": NULL"); }
        CameraOverhead = gameObject.transform.Find("CinemachineCameraOverhead").GetComponent<CinemachineCamera>();
        if (!CameraOverhead) { Debug.LogError(this.name + "/" + CameraOverhead.name + ": NULL"); }
        CameraFreefly = gameObject.transform.Find("CinemachineCameraFreefly").GetComponent<CinemachineCamera>();
        if (!CameraFreefly) { Debug.LogError(this.name + "/" + CameraFreefly.name + ": NULL"); }

    }

    private void Update()
    {
        float zoom = cameraZoom.ReadValue<float>();
        Debug.Log("zooming: " + zoom);

        if (cameraLock.WasPressedThisFrame())
        {
            cameraToggle = true;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (cameraToggle && UnityEngine.Cursor.lockState != CursorLockMode.Locked) //if wish to lock and not locked already
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            cameraToggle = false;
            Debug.Log("Cursor: LOCKED");
        }
        else if(cameraToggle && UnityEngine.Cursor.lockState == CursorLockMode.Locked) //if wish to lock and not locked already
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            cameraToggle = false;
            Debug.Log("Cursor: Confined");
        }


        /* Zooming setup
         if (zoom.ReadValue<float>() > 0.1f)   //TODO: Move to spaceship
         {
             camerasScriptable.CameraFollow.
             camerasScriptable.CameraOverhead.
             camerasScriptable.CameraFreefly.
         }
         else if (zoom.ReadValue<float>() < 0.1f)
         { 

         }*/

    }
}