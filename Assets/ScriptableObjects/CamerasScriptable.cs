using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CamerasScriptable : MonoBehaviour
{
    //TODO: implement local script debugging level
    //public bool debug = false;  //local script debug: enable/disable

    [CreateProperty] public CinemachineCamera[] AllCameras;
    private int cameraIndex = -1; //default -1: none added
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

        public DefaultSettings(params object[] parameters) : this()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is IConvertible convertible)
                {
                    if (convertible.GetTypeCode() == TypeCode.Single)
                    {
                        this.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = Convert.ToSingle(convertible);
                        return;
                    }
                    else
                    {
                        Debug.LogWarning("DefaultSettings INIT: IConvertible parameters[" + i + "] type ==" + convertible.GetTypeCode() + "; Is NOT Single !");
                    }
                }
                else if (parameters[i] is object obj)  //Vector2 most likely
                {
                    if (obj is Vector2 vector2)
                    {
                        this.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = vector2.magnitude;
                        this.DefaultDistanceAllCameras.DefaultDistanceAllCamerasVector2[i] = vector2;
                        return;
                    }
                    else
                    {
                        Debug.LogWarning("DefaultSettings INIT: object parameters[" + i + "] type ==" + obj.GetType() + "; Is object, but NOT Vector2 !");
                    }
                }
                else //NOT IConvertible or Vector2
                {
                    Debug.LogWarning("DefaultSettings INIT: parameters["+i+"] type ==" +parameters[i].GetType() + "; Needs to be IConvertible OR object!");
                }
            }
        }
    }

    //TODO: FEATURE >> Freeflycamera ability - click/select tracking targets
    //TODO: Freeflycamera movement controls / dummy gameobject as default tracking target?



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AllCameras = FindObjectsByType<CinemachineCamera>();
        Debug.Log("Added " +AllCameras.Length +" cameras.");

        defaults = new DefaultSettings();
        for (int i = 0; i < AllCameras.Length; i++)
        {
            if (AllCameras[i].TryGetComponent<CinemachineThirdPersonFollow>(out thirdPerson)) { defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = thirdPerson.CameraDistance; return; }
            if (AllCameras[i].TryGetComponent<CinemachineOrbitalFollow>(out orbital)) { defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = orbital.Radius; return; }
            if (AllCameras[i].TryGetComponent<CinemachineFollow>(out standardFollow))
            {
                defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[i] = standardFollow.FollowOffset.magnitude;
                defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasVector2[i] = standardFollow.FollowOffset;
                return;
            }
        }
        //END: Store defaults

        //START: Controls
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");  //mouse scrollwheel
        cameraLock = InputSystem.actions.FindAction("CameraLock");  //TAB
        //END: Controls

        //TODO: freefly-camera - on click to follow the object :: cinemachinecamera.trackingtarget + cinemachinepositioncontroller
    }

    void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)    //to only "GetComponent" once when camera changes - and not each zoomevent
    {
        ClearCameraCache(); //TODO: need to also clear activeCamera for edge cases? If there is no incoming camera?

        activeCamera = evt.IncomingCamera as CinemachineCamera;

        cameraIndex = Array.FindIndex(AllCameras, 0, x => x.name == activeCamera.name);
        
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
        else if (cameraToggle && UnityEngine.Cursor.lockState == CursorLockMode.Locked) //if wish to lock and not locked already
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
            Debug.Log("Zooming: Camera[" + cameraIndex + "], " + activeCamera.name +": " + zoom);
            // --- METHOD 1: Distance-based Positioning Components ---
            // Non-active cameras will be nulled by cache clearing when switching cameras
            // Check for Third Person Follow
            if (thirdPerson)
            {
                distanceScalingFactor = thirdPerson.CameraDistance / defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[cameraIndex];
                thirdPerson.CameraDistance = Mathf.Lerp(thirdPerson.CameraDistance, Mathf.Clamp(thirdPerson.CameraDistance - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Modern Orbital Follow (New FreeLook Rig mechanism)
            else if (orbital)
            {
                distanceScalingFactor = orbital.Radius / defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[cameraIndex];
                orbital.Radius = Mathf.Lerp(orbital.Radius, Mathf.Clamp(orbital.Radius - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Cinemachine Follow (Standard Transposer style position control)
            else if (standardFollow)
            {
                Vector3 offset = standardFollow.FollowOffset;
                distanceScalingFactor = offset.magnitude / defaults.DefaultDistanceAllCameras.DefaultDistanceAllCamerasFloat[cameraIndex];
                //TODO: equal magnitude adjustment of the vector instead of just using the z would maintain angle of camera to target
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