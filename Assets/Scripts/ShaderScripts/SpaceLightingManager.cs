using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpaceLightingManager : MonoBehaviour
{
    public bool debugger = false;
    private static SpaceLightingManager _instance;
    public static SpaceLightingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SpaceLightingManager>();
            }
            return _instance;
        }
    }

    private void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
            return;
        }
        _instance = this;
        RefreshSceneLists();
    }

    private void RefreshSceneLists()
    {
        _occluders.Clear();
        _occluders.AddRange(FindObjectsByType<SpaceOccluder>());
        if (debugger) Debug.Log("_occluders: " +_occluders.Count);

        _starLights.Clear();
        _starLights.AddRange(FindObjectsByType<SpaceStarLight>());
        if (debugger) Debug.Log("_starLights: " + _starLights.Count);
    }

    [Header("Tracking Collections")]
    public List<SpaceOccluder> _occluders = new List<SpaceOccluder>();
    public List<SpaceStarLight> _starLights = new List<SpaceStarLight>();

    // Constants matching the maximum array boundaries defined in the HLSL file
    private const int MAX_ELEMENTS = 16;

    // --- Cached CPU Buffers to prevent runtime GC Allocations ---
    private Vector4[] _occluderPositionsBuffer = new Vector4[MAX_ELEMENTS];
    private float[] _occluderRadiiBuffer = new float[MAX_ELEMENTS];
    private float[] _occluderSoftnessBuffer = new float[MAX_ELEMENTS];

    private Vector4[] _starPositionsBuffer = new Vector4[MAX_ELEMENTS];
    private Vector4[] _starColorsBuffer = new Vector4[MAX_ELEMENTS];

    // --- Global Shader Property IDs ---
    private static readonly int OccluderPositionsID = Shader.PropertyToID("_OccluderPositions");
    private static readonly int OccluderRadiiID = Shader.PropertyToID("_OccluderRadii");
    private static readonly int OccluderSoftnessID = Shader.PropertyToID("_OccluderSoftness");
    private static readonly int OccluderCountID = Shader.PropertyToID("_OccluderCount");

    private static readonly int StarPositionsID = Shader.PropertyToID("_StarPositions");
    private static readonly int StarColorsID = Shader.PropertyToID("_StarColors");
    private static readonly int LightCountID = Shader.PropertyToID("_LightCount");

    private void LateUpdate()
    {
        UpdateShaderBuffers();
    }

    private void UpdateShaderBuffers()
    {
        // 1. Process Shadow Casting Occluders
        int activeOccludersCount = Mathf.Min(_occluders.Count, MAX_ELEMENTS);
        for (int i = 0; i < activeOccludersCount; i++)
        {
            if (_occluders[i] != null)
            {
                _occluderPositionsBuffer[i] = _occluders[i].transform.position; // w is defaults to 0
                _occluderRadiiBuffer[i] = _occluders[i].radius;
                _occluderSoftnessBuffer[i] = _occluders[i].softness;
            }
        }

        // 2. Process Multi-Light Star Sources
        int activeLightsCount = Mathf.Min(_starLights.Count, MAX_ELEMENTS);
        for (int i = 0; i < activeLightsCount; i++)
        {
            if (_starLights[i] != null)
            {
                Vector3 pos = _starLights[i].transform.position;
                float range = _starLights[i].lightRange;
                _starPositionsBuffer[i] = new Vector4(pos.x, pos.y, pos.z, range);

                Color col = _starLights[i].starColor;
                float intensity = _starLights[i].intensity;
                _starColorsBuffer[i] = new Vector4(col.r, col.g, col.b, intensity);
            }
        }

        // 3. Dispatch Buffers Directly to global GPU memory
        Shader.SetGlobalVectorArray(OccluderPositionsID, _occluderPositionsBuffer);
        Shader.SetGlobalFloatArray(OccluderRadiiID, _occluderRadiiBuffer);
        Shader.SetGlobalFloatArray(OccluderSoftnessID, _occluderSoftnessBuffer);
        Shader.SetGlobalFloat(OccluderCountID, activeOccludersCount);

        Shader.SetGlobalVectorArray(StarPositionsID, _starPositionsBuffer);
        Shader.SetGlobalVectorArray(StarColorsID, _starColorsBuffer);
        Shader.SetGlobalFloat(LightCountID, activeLightsCount);
    }

    // --- Public Registration API Pipeline ---
    public void RegisterOccluder(SpaceOccluder occluder)
    {
        if (!_occluders.Contains(occluder)) _occluders.Add(occluder);
    }

    public void UnregisterOccluder(SpaceOccluder occluder)
    {
        if (_occluders.Contains(occluder)) _occluders.Remove(occluder);
    }

    public void RegisterStar(SpaceStarLight star)
    {
        if (!_starLights.Contains(star)) _starLights.Add(star);
    }

    public void UnregisterStar(SpaceStarLight star)
    {
        if (_starLights.Contains(star)) _starLights.Remove(star);
    }
}
