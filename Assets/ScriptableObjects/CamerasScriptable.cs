using Unity.Cinemachine;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CamerasScriptable : MonoBehaviour
{
    [CreateProperty] public CinemachineCamera CameraFollow;
    [CreateProperty] public CinemachineCamera CameraOverhead;
    [CreateProperty] public CinemachineCamera CameraFreefly;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraFollow = gameObject.transform.Find("CinemachineCameraFollow").GetComponent<CinemachineCamera>();
        if (!CameraFollow) { Debug.LogError(this.name + "/" +CameraFollow.name + ": NULL"); }
        CameraOverhead = gameObject.transform.Find("CinemachineCameraOverhead").GetComponent<CinemachineCamera>();
        if (!CameraOverhead) { Debug.LogError(this.name + "/" + CameraOverhead.name + ": NULL"); }
        CameraFreefly = gameObject.transform.Find("CinemachineCameraFreefly").GetComponent<CinemachineCamera>();
        if (!CameraFreefly) { Debug.LogError(this.name + "/" + CameraFreefly.name + ": NULL"); }
    }

    // Update is called once per frame
    void Update()
    {

    }
}