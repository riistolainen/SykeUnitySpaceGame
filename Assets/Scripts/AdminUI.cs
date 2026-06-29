//ATTACHED TO UIDOCUMENT -obj

using System.Linq;
using UnityEditor.Rendering;

//using UnityEditor.Toolbars;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]  //Not critical, but recommended to check the object has the UIDocument as a component

public class AdminUIScript : MonoBehaviour
{

    CamerasScriptable camerasScriptable;    //ref to our scriptableobject

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GET REF TO CAMERAS.obj
        GameObject camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }
        camerasScriptable = camRef.GetComponent<CamerasScriptable>();
        if (camerasScriptable == null) { Debug.LogError("FAILED INIT: camerasScriptable NULL"); }

        //GET REF TO UI-element
        UIDocument uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        RadioButtonGroup rbg = root.Q<RadioButtonGroup>("CameraSwitcher"); //'#' required or not in selector specification?
                                                                           //TODO: rbg does not get the active reference - returns with 0 children while 3 are present in game
        Debug.Log("LOOKING-base: " + rbg + ", " + rbg.name + ", " + rbg.childCount + ", " + rbg.GetBindingInfos() + ", " + rbg.choices + ", " + rbg.value + ", " + rbg.Children().Count());
        rbg.RegisterValueChangedCallback(RBGToggleEvent);

        //only is the string
        //Debug.Log("LOOKING-[0]: " + rbg.choices.ElementAt(0) + ", " + rbg.choices.ElementAt(0).name + ", " + rbg.choices.ElementAt(0).childCount() + ", " + rbg.choices.ElementAt(0).GetBindingInfos() + ", ");
        //outofrange
        //Debug.Log("LOOKING-[0]: " + rbg[0] + ", " + rbg[0].name + ", " + rbg[0].childCount + ", " + rbg[0].GetBindingInfos() + ", ");

        /*
        //Add binding between UI and scriptableObject
        rbg.ElementAt(0).dataSource = camerasScriptable.CameraFollow;
        rbg.ElementAt(1).dataSource = camerasScriptable.CameraOverhead;
        rbg.ElementAt(2).dataSource = camerasScriptable.CameraFreefly;
        */
        /*
        var binding0 = new DataBinding
        {
            dataSource = rbg,
            dataSourcePath = PropertyPath.FromName(rbg.ElementAt(0).name),
        };
        var binding1 = new DataBinding
        {
            dataSource = rbg,
            dataSourcePath = PropertyPath.FromName(rbg.ElementAt(1).name),
        };
        var binding2 = new DataBinding
        {
            dataSource = rbg,
            dataSourcePath = PropertyPath.FromName(rbg.ElementAt(2).name),
        };
        
        rbg[0].SetBinding("camFo", binding0);
        rbg[1].SetBinding("camOv", binding1);
        rbg[2].SetBinding("camFr", binding2);
        */

    }



    /*
    //Callback registry
    //For some reason can't register at RBGroup-level event - instead adding individual RB-level
    //rbg.RegisterValueChangedCallback<ChangeEvent<int>>(RBGToggle);

    rbg.ElementAt(0).RegisterCallback<ChangeEvent<Toggle>>(Followevt);
    rbg.ElementAt(1).RegisterCallback<ChangeEvent<Toggle>>(Overheadevt);
    rbg.ElementAt(2).RegisterCallback<ChangeEvent<Toggle>>(Freeflyevt);
}
*/
    private void FixedUpdate()
    {
        
    }


    //TODO: React to UI change through methods and update which camera is active?
    void RBGToggleEvent(ChangeEvent<int> evt)
    {
        Debug.Log("RBGToggleEvent: " + evt.target + " " + evt.previousValue + " " + evt.newValue + " " + evt.ToString() +" Listlngt: " + camerasScriptable.AllCameras.Count +" name: " + camerasScriptable.AllCameras[evt.newValue].name);

        camerasScriptable.AllCameras[evt.newValue].Priority = 1;
        camerasScriptable.AllCameras[evt.previousValue].Priority = 0;
    }

    /*
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
    */
}