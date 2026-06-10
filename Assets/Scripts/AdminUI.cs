//ATTACHED TO UIDOCUMENT -obj

using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AdminUIScript : MonoBehaviour
{

    CamerasScriptable camerasScriptable;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject camRef = GameObject.Find("Cameras");
        if(camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }
        camerasScriptable = camRef.GetComponent<CamerasScriptable>();
        if (camerasScriptable == null){ Debug.LogError("FAILED INIT: camerasScriptable NULL"); }

        UIDocument uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        //        VisualTreeAsset vTA = uiDocument.visualTreeAsset;
        RadioButtonGroup rbg = root.Q<RadioButtonGroup>("CameraSwitcher");
        Debug.Log("LOOKING: " +rbg +", " +rbg.name +", " +rbg.childCount +", " +rbg.GetBindingInfos() +", " +rbg.choices);
        rbg.ElementAt(0).dataSource = camerasScriptable.CameraFollow;
        rbg.ElementAt(1).dataSource = camerasScriptable.CameraOverhead;
        rbg.ElementAt(2).dataSource = camerasScriptable.CameraFreefly;

        //Callback registry
        /*For some reason can't register at RBGroup-level event - instead adding individual RB-level*/
        //rbg.RegisterValueChangedCallback<ChangeEvent<int>>(RBGToggle);

        rbg.ElementAt(0).RegisterCallback<ChangeEvent<Toggle>>(Followevt);
        rbg.ElementAt(1).RegisterCallback<ChangeEvent<Toggle>>(Overheadevt);
        rbg.ElementAt(2).RegisterCallback<ChangeEvent<Toggle>>(Freeflyevt);
    }


    //TODO: React to UI change through methods and update which camera is active?
    void RBGToggle(ChangeEvent<int> evt)
    {
        //do
    
    }

    void Followevt(ChangeEvent<Toggle> evt)
    {         //TODO this might be complete wrong setup
        Debug.Log("Followevt: " +evt.newValue);


    }

    void Overheadevt(ChangeEvent<Toggle> evt)
    {        //TODO this might be complete wrong setup
        Debug.Log("Followevt: " + evt.newValue);

    }

    void Freeflyevt(ChangeEvent<Toggle> evt)
    {        //TODO this might be complete wrong setup
        Debug.Log("Followevt: " + evt.newValue);
    }

    private void OnBoolChangedEvent(ChangeEvent<bool> evt)
    {
        Debug.Log($"Toggle changed. Old value: {evt.previousValue}, new value: {evt.newValue}");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
