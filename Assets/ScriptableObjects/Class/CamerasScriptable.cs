using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "CamerasScriptable", menuName = "Scriptable Objects/CamerasScriptable")]
public class CamerasScriptableScript : ScriptableObject
{
    [SerializeField] private CinemachineCamera _cameraFollow;   //the in-between
    [SerializeField] private CinemachineCamera _cameraOverhead;
    [SerializeField] private CinemachineCamera _cameraFreefly;

    [SerializeField] private CinemachineCamera[] _cameraArray;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraArray = new CinemachineCamera[3];
        _cameraArray[0] = _cameraFollow;
        _cameraArray[1] = _cameraOverhead;
        _cameraArray[2] = _cameraFreefly;
    }


    // Update is called once per frame
    void Update()
    {

    }
}