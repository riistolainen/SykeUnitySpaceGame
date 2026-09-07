//ATTACHED TO UIDOCUMENT -obj

using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]  //Not critical, but recommended to check the object has the UIDocument as a component

public class AdminUIScript : MonoBehaviour
{
    CamerasScript camerasScript;    //TODO: Am I using scriptable object or not? "ref to our scriptableobject" --not? reference to CamerasScript

    GameObject camRef;
    UIDocument uiDocument;
    VisualElement root;
    RadioButtonGroup rbg;
    Slider sliderTimeScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GET REF TO CAMERAS.obj
        camRef = GameObject.Find("Cameras");
        if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); }
        if (!camRef.TryGetComponent<CamerasScript>(out camerasScript)) { Debug.LogError("FAILED INIT: camerasScript NULL"); }

        //GET REF TO UI-element
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        rbg = root.Q<RadioButtonGroup>("CameraSwitcher"); //'#' required or not in selector specification?
                                                                           //TODO: rbg does not get the active reference - returns with 0 children while 3 are present in game
        Debug.Log("LOOKING-base: " + rbg + ", " + rbg.name + ", " + rbg.childCount + ", " + rbg.GetBindingInfos() + ", " + rbg.choices + ", " + rbg.value + ", " + rbg.Children().Count());
        rbg.RegisterValueChangedCallback(RBGToggleEvent);

        sliderTimeScale = root.Q<Slider>("TimeScale");
        sliderTimeScale.RegisterValueChangedCallback(SliderTimeScaleChangedEvent);
    }

    private void OnDisable()
    {
        rbg.UnregisterValueChangedCallback(RBGToggleEvent);
        sliderTimeScale.UnregisterValueChangedCallback(SliderTimeScaleChangedEvent);
    }

    void SliderTimeScaleChangedEvent(ChangeEvent<float> evt)
    {
        Debug.Log("SliderTimeScaleChangedEvent: " + evt.previousValue + " " + evt.newValue + " " + evt.ToString());
        Time.timeScale = evt.newValue;  //TODO: causes jerky motion on small values?
        //Time.fixedDeltaTime = 0.2f * Time.timeScale;  //TODO: causes jerky motion immediately
    }

    //TODO: React to UI change through methods and update which camera is active?
    void RBGToggleEvent(ChangeEvent<int> evt)
    {
        Debug.Log("RBGToggleEvent: " + evt.target + " " + evt.previousValue + " " + evt.newValue + " " + evt.ToString() +" Listlnght: " + camerasScript.AllCameras.Count() +" name: " + camerasScript.AllCameras[evt.newValue].name);

        camerasScript.AllCameras[evt.newValue].Priority = 1;
        camerasScript.AllCameras[evt.previousValue].Priority = 0;
    }
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

//only is the string
//Debug.Log("LOOKING-[0]: " + rbg.choices.ElementAt(0) + ", " + rbg.choices.ElementAt(0).name + ", " + rbg.choices.ElementAt(0).childCount() + ", " + rbg.choices.ElementAt(0).GetBindingInfos() + ", ");
//outofrange
//Debug.Log("LOOKING-[0]: " + rbg[0] + ", " + rbg[0].name + ", " + rbg[0].childCount + ", " + rbg[0].GetBindingInfos() + ", ");

/*
//Add binding between UI and scriptableObject
rbg.ElementAt(0).dataSource = camerasScript.CameraFollow;
rbg.ElementAt(1).dataSource = camerasScript.CameraOverhead;
rbg.ElementAt(2).dataSource = camerasScript.CameraFreefly;
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