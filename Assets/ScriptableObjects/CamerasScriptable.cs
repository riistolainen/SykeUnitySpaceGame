using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CamerasScriptable : MonoBehaviour
{
    [CreateProperty] public CinemachineCamera[] AllCameras;
    [CreateProperty] public CinemachineCamera CameraFollow;
    [CreateProperty] public CinemachineCamera CameraOverhead;
    [CreateProperty] public CinemachineCamera CameraFreefly;
    [CreateProperty] public CinemachineCamera CameraCockpit;

    public InputAction cameraZoom;
    public InputAction cameraLock;
    private bool cameraToggle = false;
    private bool cameraZoomed = false;
    private float distanceScalingFactor = 1f;

    DefaultSettings defaults;

    [SerializeField] private float zoom = 0;
    [SerializeField] private CinemachineCamera activeCamera;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 100f;

    private CinemachineThirdPersonFollow thirdPerson;
    private CinemachineOrbitalFollow orbital;
    private CinemachineFollow standardFollow;

    public class DefaultDistanceCameraSettings
    {
        public float[] DefaultDistanceAllCamerasFloat;
        public Vector2[] DefaultDistanceAllCamerasVector2;  //needs separate arrays for different type of values
    }

    public struct DefaultSettings
    {
        public DefaultDistanceCameraSettings DefaultDistanceAllCameras { get; set; }    //TODO: Pull Vector2 apart before sending over to other objects - does not play well with IConvertible
        
        public DefaultSettings(params IConvertible[] parameters) : this()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetTypeCode() == TypeCode.Single)
                {
                    this.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = Convert.ToSingle(parameters[i]);
                    return;
                }
                if(parameters[i].GetTypeCode() == TypeCode.Object)  //Vector2 most likely
                {
                    this.DefaultDistanceAllCameras.DefaultDistanceAllCamerasVector2[i] = (Vector2)parameters[i];

                }
                }
            }
        }
    }

    //TODO: FEATURE >> Freeflycamera ability - click/select tracking targets
    //TODO: Freeflycamera movement controls / dummy gameobject as default tracking target?

    private void OnEnable()
    {
        CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
    }
    private void OnDisable()
    {
        CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //START: Initialize cameras list
        AllCameras = FindObjectsByType<CinemachineCamera>(); //TODO: dynamically add cameras? Currently needs manual script editing if new camera is added in scene editor.
        /*CameraFollow = gameObject.transform.Find("CinemachineCameraFollow").GetComponent<CinemachineCamera>();
        if (!CameraFollow) { Debug.LogError(this.name + "/" + CameraFollow.name + ": NULL"); }
        CameraOverhead = gameObject.transform.Find("CinemachineCameraOverhead").GetComponent<CinemachineCamera>();
        if (!CameraOverhead) { Debug.LogError(this.name + "/" + CameraOverhead.name + ": NULL"); }
        CameraFreefly = gameObject.transform.Find("CinemachineCameraFreefly").GetComponent<CinemachineCamera>();
        if (!CameraFreefly) { Debug.LogError(this.name + "/" + CameraFreefly.name + ": NULL"); }
        CameraCockpit = gameObject.transform.Find("CinemachineCameraCockpit").GetComponent<CinemachineCamera>();
        if (!CameraCockpit) { Debug.LogError(this.name + "/" + CameraCockpit.name + ": NULL"); }
        */
        //END: Initialize cameras list

        //START: Store defaults
        DefaultSettings defaults = new DefaultSettings();
        for (int i = 0; i < AllCameras.Length; i++) {
            if (AllCameras[i].TryGetComponent<CinemachineThirdPersonFollow>(out thirdPerson)) { defaults.DefaultDistanceAllCameras[i] = thirdPerson.CameraDistance; return; }
            if (AllCameras[i].TryGetComponent<CinemachineOrbitalFollow>(out orbital)) { defaults.DefaultDistanceAllCameras[i] = orbital.Radius; return; }
            if (AllCameras[i].TryGetComponent<CinemachineFollow>(out standardFollow)) {
                defaults.DefaultDistanceAllCameras[i] = -1f;
                defaults.custom[i] = standardFollow.FollowOffset;
                return;
        }
        //END: Store defaults

        //START: Controls
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");  //mouse scrollwheel
        cameraLock = InputSystem.actions.FindAction("CameraLock");  //TAB
        //END: Controls

        //TODO: freefly-camera - on click to follow the object :: cinemachinecamera.trackingtarget + cinemachinepositioncontroller

        //TODO: NOT USED?
        /*GET REF TO CAMERAS.obj
        GameObject camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }
        */
    }

    void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)    //to only "GetComponent" once when camera changes - and not each zoomevent
    {
        ClearCameraCache(); //TODO: need to also clear activeCamera for edge cases? If there is no incoming camera?

        activeCamera = evt.IncomingCamera as CinemachineCamera;
        if (activeCamera.TryGetComponent<CinemachineThirdPersonFollow>(out thirdPerson)) { return; }
        if (activeCamera.TryGetComponent<CinemachineOrbitalFollow>(out orbital)) { return; }
        if (activeCamera.TryGetComponent<CinemachineFollow>(out standardFollow)) { return; }
    }
    void ClearCameraCache()
    {
        thirdPerson = null;
        orbital = null;
        standardFollow = null;
    }


    private void Update()
    {
        if (cameraZoom.triggered)
        {
            zoom = -cameraZoom.ReadValue<float>();
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
        //TODO: move getcomponents to start so they are not ran everytime
        if (cameraZoomed)
        {
            cameraZoomed = false;

            Debug.Log("zooming " + activeCamera.name + ": " + zoom);
            // --- METHOD 1: Distance-based Positioning Components ---
            // Non-active cameras will be nulled by cache clearing when switching cameras
            // Check for Third Person Follow
            if (thirdPerson)
            {
                distanceScalingFactor = thirdPerson.CameraDistance / defaults.DefaultDistanceCameraFollow;
                thirdPerson.CameraDistance = Mathf.Lerp(thirdPerson.CameraDistance, Mathf.Clamp(thirdPerson.CameraDistance - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Modern Orbital Follow (New FreeLook Rig mechanism)
            else if (orbital)
            {
                orbital.Radius = Mathf.Lerp(orbital.Radius, Mathf.Clamp(orbital.Radius - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Cinemachine Follow (Standard Transposer style position control)
            else if (standardFollow)
            {
                Vector3 offset = standardFollow.FollowOffset;
                // Zoom by scaling the Z offset (or adjust magnitude evenly)
                offset.z = Mathf.Lerp(offset.z, Mathf.Clamp(offset.z + (zoom * zoomSpeed * distanceScalingFactor), -maxZoom, -minZoom), Time.deltaTime * 5f);
                standardFollow.FollowOffset = offset;
            }

            // --- METHOD 2: Lens fallbacks (FOV / Orthographic Size) ---
            // If no distance-based component is active, zoom the internal camera lens directly.
            else
            {
                LensSettings lens = activeCamera.Lens;
                if (lens.Orthographic)
                {
                    lens.OrthographicSize = Mathf.Lerp(lens.OrthographicSize, Mathf.Clamp(lens.OrthographicSize - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
                }
                else
                {
                    lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, Mathf.Clamp(lens.FieldOfView - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
                }
                activeCamera.Lens = lens; // Reassign struct back to property
            }
        }
    }
}