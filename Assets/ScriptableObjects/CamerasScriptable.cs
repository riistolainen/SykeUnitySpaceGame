using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CamerasScriptable", menuName = "Scriptable Objects/CamerasScriptable")]
public class CamerasScriptable : ScriptableObject
{
    public CinemachineCamera CameraFollow;
    public CinemachineCamera CameraOverhead;
    public CinemachineCamera CameraFreefly;
    
}
