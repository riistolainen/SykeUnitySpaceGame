using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class CamerasScriptable : MonoBehaviour
{
    [CreateProperty] public List<CinemachineCamera> AllCameras;
    [CreateProperty] public CinemachineCamera CameraFollow;
    [CreateProperty] public CinemachineCamera CameraOverhead;
    [CreateProperty] public CinemachineCamera CameraFreefly;
    [CreateProperty] public CinemachineCamera CameraCockpit;

    public InputAction cameraZoom;
    public InputAction cameraLock;
    private bool cameraToggle = false;
    private bool cameraZoomed = false;

    private float zoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AllCameras.AddRange(new List<CinemachineCamera>{ CameraFollow, CameraOverhead, CameraFreefly, CameraCockpit} );

        //Controls
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");  //mouse scrollwheel
        cameraLock = InputSystem.actions.FindAction("CameraLock");  //TAB

        zoom = 0;

        //TODO: freefly-camera - on click to follow the object :: cinemachinecamera.trackingtarget + cinemachinepositioncontroller

        //GET REF TO CAMERAS.obj
        GameObject camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }

        CameraFollow = gameObject.transform.Find("CinemachineCameraFollow").GetComponent<CinemachineCamera>();
        if (!CameraFollow) { Debug.LogError(this.name + "/" +CameraFollow.name + ": NULL"); }
        CameraOverhead = gameObject.transform.Find("CinemachineCameraOverhead").GetComponent<CinemachineCamera>();
        if (!CameraOverhead) { Debug.LogError(this.name + "/" + CameraOverhead.name + ": NULL"); }
        CameraFreefly = gameObject.transform.Find("CinemachineCameraFreefly").GetComponent<CinemachineCamera>();
        if (!CameraFreefly) { Debug.LogError(this.name + "/" + CameraFreefly.name + ": NULL"); }
        CameraCockpit = gameObject.transform.Find("CinemachineCameraCockpit").GetComponent<CinemachineCamera>();
        if (!CameraCockpit) { Debug.LogError(this.name + "/" + CameraCockpit.name + ": NULL"); }
    }


    private void Update()
    {
        if (cameraZoom.triggered) 
        {
        zoom =+ cameraZoom.ReadValue<float>();
        Debug.Log("zooming: " + Mouse.current.scroll.ReadValue().y + ", " + zoom);
        }

        if (cameraZoom.WasPressedThisFrame())
        {
            cameraZoomed = true;
            Debug.Log("cameraZoomed:true");
        }

        if (cameraLock.WasPressedThisFrame())
        {
            cameraToggle = true;
        }
    }

    private void FixedUpdate()
    {
        //Cursor locking --->>
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
        //<<--- END:Cursor locking


        //CameraZooming --->>
        if (cameraZoomed)
        {
            cameraZoomed = false;

            CinemachineCamera activeCamera =GetComponentInChildren<CinemachineBrain>().ActiveVirtualCamera as CinemachineCamera;
            if (activeCamera)
            {//TODO fix for each different camera. Currently global coordinates movement to z - not away from current forward
                activeCamera.transform.position = new Vector3(activeCamera.transform.localPosition.x, activeCamera.transform.localPosition.y, activeCamera.transform.localPosition.z+ zoom);
            }
            
        }
    }
}