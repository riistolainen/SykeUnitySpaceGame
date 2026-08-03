using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CamerasScriptable : MonoBehaviour
{
    //TODO: implement local script debugging level

    public Camera mainCamera;
    public CinemachineBrain brain;
    public ICinemachineCamera iCamera;
    public CinemachineBrainEvents brainEvents;

    public CinemachineCamera[] AllCameras;
    private int cameraIndex = -1; //default -1: none added
    public CinemachineCamera CameraCockpit;
    public CinemachineCamera CameraFollow;
    public CinemachineCamera CameraFreefly;
    public CinemachineCamera CameraOverhead;

    public InputAction cameraZoom;
    private bool cameraZoomed = false;
    public InputAction cameraLock;
    private bool cameraToggle = false;
    private float distanceScalingFactor = 1f;

    private float zoom = 0;
    private CinemachineCamera activeCamera;
    private float zoomSpeed = 10f;
    private float minZoom = 1f;
    private float maxZoom = 100f;

    private CinemachineThirdPersonFollow thirdPerson;
    private CinemachineOrbitalFollow orbital;
    private CinemachineFollow standardFollow;

    public DefaultSettings defaults;

    public struct DefaultDistanceCameraSettings
    {
        public float[] DefaultDistanceAllCamerasFloat;
        public Vector2[] DefaultDistanceAllCamerasVector2;  //needs separate arrays for different type of values

        public DefaultDistanceCameraSettings(int numberOfCameras) : this()
        {
            this.DefaultDistanceAllCamerasFloat = new float[numberOfCameras];
            this.DefaultDistanceAllCamerasVector2 = new Vector2[numberOfCameras];
        }
    }

    public struct DefaultSettings
    {
        public DefaultDistanceCameraSettings DefaultDistanceCameraSettings { get; set; }

        public DefaultSettings(CinemachineCamera[] cameraList) : this()
        {
            DefaultDistanceCameraSettings = new DefaultDistanceCameraSettings(cameraList.Length);   //arrays need to be defined before use and lists are not performant

            for (int i = 0; i < cameraList.Length; i++)
            {
                if (cameraList[i].TryGetComponent<CinemachineThirdPersonFollow>(out CinemachineThirdPersonFollow thirdPerson)) { this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = thirdPerson.CameraDistance; }
                else if (cameraList[i].TryGetComponent<CinemachineOrbitalFollow>(out CinemachineOrbitalFollow orbital)) { this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = orbital.Radius; }
                else if (cameraList[i].TryGetComponent<CinemachineFollow>(out CinemachineFollow standardFollow))   //Vector2 so let's store the actual direction also; magnitude for simple scaling conversion
                {
                    this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = standardFollow.FollowOffset.magnitude;
                    this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasVector2[i] = standardFollow.FollowOffset;
                }
                else
                {
                    LensSettings lens = cameraList[i].Lens;
                    if (lens.Orthographic) { this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = lens.OrthographicSize; }
                    else { this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = lens.FieldOfView; }
                }
            }
            /* OLD struct creator -->
            for (int i = 0; i < cameraList.Length; i++)
            {
                if (cameraList[i] is IConvertible convertible)
                {
                    if (convertible.GetTypeCode() == TypeCode.Single)
                    {
                        this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = Convert.ToSingle(convertible);
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
                        this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[i] = vector2.magnitude;
                        this.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasVector2[i] = vector2;
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
            }*/
        }
    }

    //TODO: FEATURE >> Freeflycamera ability - click/select tracking targets
    //TODO: Freeflycamera movement controls / dummy gameobject as default tracking target?

    /*
    private void OnEnable()
    {
        CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivation);
    }
    private void OnDisable()
    {
        CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivation);
    }
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivation);

        activeCamera = CameraOverhead;
        //CameraOverhead.Priority = 1;
        //TODO: Causes controls to bug out? WTFH?!
        AllCameras = FindObjectsByType<CinemachineCamera>().OrderBy(x => x.name).ToArray();
        Debug.Log("Added " + AllCameras.Length + " cameras.");
        defaults = new DefaultSettings(AllCameras);

        //END: Store defaults

        //START: Controls
        cameraZoom = InputSystem.actions.FindAction("CameraZoom");  //mouse scrollwheel
        cameraLock = InputSystem.actions.FindAction("CameraLock");  //TAB
        //TODO: only allow manual camera control when cursor is locked, if unlocked - assumed that user wants to interact with UI
        //END: Controls


        /*
        //DEBUG-CONTROLS
        InputActionTrace trace = new InputActionTrace();
        trace.SubscribeTo(cameraZoom);

        // Record a single triggering of an action.
        cameraZoom.performed += ctx =>
        {
            if (ctx.ReadValue<float>() > 0.5f)
                trace.RecordAction(ctx);
        };
        // Output trace to console.
        Debug.Log(string.Join(",\n", trace));

        //END: DEBUG-CONTROLS
        */

        //TODO: freefly-camera - on click to follow the object :: cinemachinecamera.trackingtarget + cinemachinepositioncontroller
    }

    void OnCameraActivation(ICinemachineCamera.ActivationEventParams evt)    //to only "GetComponent" once when camera changes - and not each zoomevent
    {
        Debug.Log("EVENT: OnCameraActivated: Brain.ActiveCamera.Name: " + evt.OutgoingCamera.Name + ", Camera: " + evt.IncomingCamera.Name);
        ClearCameraCache();
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
        if (cameraZoom.WasPressedThisFrame())
        {
            zoom = -cameraZoom.ReadValue<float>();
            cameraZoomed = true;
            Debug.Log("cameraZoomed:true");
        }

        if (cameraLock.WasPressedThisFrame())
        {
            cameraToggle = true;
            Debug.Log("cameraLock:true");
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
        if (cameraZoomed)
        {
            cameraZoomed = false;
            Debug.Log("Zooming: Camera[" + cameraIndex + "], " + activeCamera.name + ": " + zoom);
            // --- METHOD 1: Distance-based Positioning Components ---
            // Non-active cameras will be nulled by cache clearing when switching cameras
            // Check for Third Person Follow
            if (thirdPerson)
            {
                distanceScalingFactor = thirdPerson.CameraDistance / defaults.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[cameraIndex];
                thirdPerson.CameraDistance = Mathf.Lerp(thirdPerson.CameraDistance, Mathf.Clamp(thirdPerson.CameraDistance - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Modern Orbital Follow (New FreeLook Rig mechanism)
            else if (orbital)
            {
                distanceScalingFactor = orbital.Radius / defaults.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[cameraIndex];
                orbital.Radius = Mathf.Lerp(orbital.Radius, Mathf.Clamp(orbital.Radius - (zoom * zoomSpeed * distanceScalingFactor), minZoom, maxZoom), Time.deltaTime * 5f);
            }

            // Check for Cinemachine Follow (Standard Transposer style position control)
            else if (standardFollow)
            {
                Vector3 offset = standardFollow.FollowOffset;
                distanceScalingFactor = offset.magnitude / defaults.DefaultDistanceCameraSettings.DefaultDistanceAllCamerasFloat[cameraIndex];
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