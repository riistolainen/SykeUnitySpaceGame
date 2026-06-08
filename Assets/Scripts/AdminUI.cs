//ATTACHED TO UIDOCUMENT -obj

using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AdminUIScript : MonoBehaviour
{

    CamerasScriptable camerasScriptable;

    private void OnTestToggleChanged(ChangeEvent<bool> evt)
    {
        // Handling code
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camerasScriptable = transform.parent.parent.Find("Cameras").GetComponent<CamerasScriptable>();
        
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualTreeAsset vTA = uiDocument.GetComponent<VisualTreeAsset>();
        RadioButtonGroup rbg = vTA.GetComponent<RadioButtonGroup>();

        //rbg.ElementAt(0).Bind(camerasScriptable.CameraFollow);
        //rbg.ElementAt(0).SetBinding(BindingId );
        rbg.ElementAt(0).dataSource = camerasScriptable.CameraFollow;
        rbg.ElementAt(1).dataSource = camerasScriptable.CameraOverhead;
        rbg.ElementAt(2).dataSource = camerasScriptable.CameraFreefly;

        rbg.ElementAt(0).RegisterCallback<ChangeEvent<Toggle>> (Myevt);
        rbg.RegisterValueChangedCallback(evt => { });
    }

    //TODO: React to UI change through methods and update which camera is active?
    void Myevt(ChangeEvent<Toggle> evt)
    { 
        //TODO this might be complete wrong setup
    
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
