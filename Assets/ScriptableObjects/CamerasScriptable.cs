using Unity.Cinemachine;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CamerasScriptable : MonoBehaviour
{
    [CreateProperty] public CamerasScriptable camerasScriptable;

    [CreateProperty] public CinemachineCamera CameraFollow;     //realdeal
    [SerializeField] private CinemachineCamera _cameraFollow;   //the in-between

    [CreateProperty] public CinemachineCamera CameraOverhead;
    [SerializeField] private CinemachineCamera _cameraOverhead;

    [CreateProperty] public CinemachineCamera CameraFreefly;
    [SerializeField] private CinemachineCamera _cameraFreefly;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GET REF TO CAMERAS.obj
        GameObject camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }
        camerasScriptable = camRef.GetComponent<CamerasScriptable>();
        if (camerasScriptable == null) { Debug.LogError("FAILED INIT: camerasScriptable NULL"); }


        CameraFollow = gameObject.transform.Find("CinemachineCameraFollow").GetComponent<CinemachineCamera>();
        if (!CameraFollow) { Debug.LogError(this.name + "/" +CameraFollow.name + ": NULL"); }
        CameraOverhead = gameObject.transform.Find("CinemachineCameraOverhead").GetComponent<CinemachineCamera>();
        if (!CameraOverhead) { Debug.LogError(this.name + "/" + CameraOverhead.name + ": NULL"); }
        CameraFreefly = gameObject.transform.Find("CinemachineCameraFreefly").GetComponent<CinemachineCamera>();
        if (!CameraFreefly) { Debug.LogError(this.name + "/" + CameraFreefly.name + ": NULL"); }
    
        camerasScriptable.CameraFollow = CameraFollow;
        camerasScriptable.CameraOverhead = CameraOverhead;
        camerasScriptable.CameraFreefly = CameraFreefly;
    }

    // Update is called once per frame
    void Update()
    {

    }
}