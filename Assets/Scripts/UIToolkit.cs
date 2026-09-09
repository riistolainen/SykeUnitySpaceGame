using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class UIToolkit : MonoBehaviour
{
    [SerializeField] private CamerasScript camerasScript; // Recommended: Assign via Inspector if possible

    private GameObject camRef;
    private PanelRenderer m_PanelRenderer;
    private int m_CurrentUIVersion = -1;

    private RadioButtonGroup rbg;
    private Slider sliderTimeScale;

    private void Awake()
    {
        // 1. GET REF TO CAMERAS.obj
        if (camerasScript == null)
        {
            camRef = GameObject.Find("Cameras");
            if (camRef == null) { Debug.LogError("FAILED INIT: camRef NULL"); return; }
            if (!camRef.TryGetComponent<CamerasScript>(out camerasScript)) { Debug.LogError("FAILED INIT: camerasScript NULL"); }
        }
    }

    private void OnEnable()
    {
        m_PanelRenderer = GetComponent<PanelRenderer>();

        // Subscribe to the modern PanelRenderer reload lifecycle loop
        m_PanelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        if (m_PanelRenderer != null)
        {
            m_PanelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        // Clean up UI events safely
        UnsubscribeUIEvents();
    }

    // Modern replacement for UI setup logic
    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        // Guard Clause: Avoid double-binding or redundant processing if UI didn't actually rebuild
        if (m_CurrentUIVersion == version) return;

        // Clean up old subscriptions before re-binding to new layout elements
        UnsubscribeUIEvents();
        m_CurrentUIVersion = version;

        // 2. GET REF TO UI-elements (Do NOT use '#' in root.Q selectors)
        rbg = root.Q<RadioButtonGroup>("CameraSwitcher");
        sliderTimeScale = root.Q<Slider>("TimeScale");

        if (rbg != null)
        {
            // The tree is fully built here: child counts and choices will now resolve correctly!
            Debug.Log($"UI Loaded - RadioButtonGroup found. Choices count: {rbg.choices.Count()}");

            rbg.RegisterValueChangedCallback(RBGToggleEvent);
        }
        else
        {
            Debug.LogError("FAILED INIT: CameraSwitcher RadioButtonGroup not found in Visual Tree.");
        }

        if (sliderTimeScale != null)
        {
            sliderTimeScale.RegisterValueChangedCallback(SliderTimeScaleChangedEvent);
        }
        else
        {
            Debug.LogError("FAILED INIT: TimeScale Slider not found in Visual Tree.");
        }
    }

    private void UnsubscribeUIEvents()
    {
        if (rbg != null) rbg.UnregisterValueChangedCallback(RBGToggleEvent);
        if (sliderTimeScale != null) sliderTimeScale.UnregisterValueChangedCallback(SliderTimeScaleChangedEvent);
    }

    private void SliderTimeScaleChangedEvent(ChangeEvent<float> evt)
    {
        Debug.Log($"SliderTimeScaleChangedEvent: {evt.previousValue} -> {evt.newValue}");
        Time.timeScale = evt.newValue;

        // Pro-tip for your jerking issue: If modifying timeScale heavily, you must update fixedDeltaTime proportionately
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void RBGToggleEvent(ChangeEvent<int> evt)
    {
        if (camerasScript == null || camerasScript.AllCameras == null) return;

        // Boundary checks to prevent index out of range exceptions
        if (evt.newValue >= 0 && evt.newValue < camerasScript.AllCameras.Count())
        {
            camerasScript.AllCameras[evt.newValue].Priority = 1;
        }

        if (evt.previousValue >= 0 && evt.previousValue < camerasScript.AllCameras.Count())
        {
            camerasScript.AllCameras[evt.previousValue].Priority = 0;
        }

        Debug.Log($"Camera toggled to index: {evt.newValue}");
    }
}
