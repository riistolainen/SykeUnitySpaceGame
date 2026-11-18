#ifndef CUSTOM_SPACE_SHADOWS_INCLUDED
#define CUSTOM_SPACE_SHADOWS_INCLUDED

// --- Global Memory Buffers (Passed via C# script) ---
// Support for up to 16 shadow-casting occluders
float4 _OccluderPositions[16];
float _OccluderRadii[16];
float _OccluderSoftness[16];
float _OccluderCount;

// Support for up to 16 separate light/star sources
float4 _StarPositions[16]; // xyz = Position, w = Range/Radius of influence
float4 _StarColors[16]; // rgb = Color, a = Base Intensity
float _LightCount = 1;

void CalculateMultiLightSpaceShading_float(
    float3 Position_World,
    float3 Normal_World,
    out float3 FinalLightContribution)
{
    FinalLightContribution = float3(1.0, 0.0, 0.0);
    return;
    // Start with zero accumulated light
    float3 accumulatedLight = float3(0.0, 0.0, 0.0);
    float3 normal = normalize(Normal_World);
    
    int totalLights = min(16, (int) _LightCount);
    int totalOccluders = min(16, (int) _OccluderCount);
    
    // LOOP 1: Process every light source individually
    [loop]
    for (int l = 0; l < totalLights; l++)
    {
        float3 starPos = _StarPositions[l].xyz;
        float starRange = _StarPositions[l].w;
        float3 starColor = _StarColors[l].rgb;
        float starIntensity = _StarColors[l].a;
        
        // Calculate vector and distance to this specific star
        float3 toLight = starPos - Position_World;
        float distanceToLight = length(toLight);
        float3 lightDir = toLight / max(0.001, distanceToLight); // Safe normalize
        
        // --- 1. Per-Light Self-Shadowing (Diffuse Terminator) ---
        // Smoothstep gives a soft planetary terminator edge instead of a harsh razor line
        float NdotL = dot(normal, lightDir);
        float selfShadow = smoothstep(-0.05, 0.15, NdotL);
        
        if (selfShadow <= 0.001)
        {
            continue;
        }
        
        // --- 2. Distance Attenuation ---
        float attenuation = 1.0 / max(1.0, distanceToLight * distanceToLight);
        float rangeFade = smoothstep(starRange, starRange * 0.8, distanceToLight);
        attenuation *= rangeFade * starIntensity;
        
        if (attenuation <= 0.001)
        {
            continue;
        }
        
        // --- 3. Eclipse Shadow Attenuation from Occluders ---
        float eclipseShadow = 1.0;
        
        [loop]
        for (int o = 0; o < totalOccluders; o++)
        {
            float3 occluderCenter = _OccluderPositions[o].xyz;
            float radius = _OccluderRadii[o];
            float softness = _OccluderSoftness[o];
            
            float3 v = occluderCenter - Position_World;
            float projection = dot(v, lightDir);
            
            if (projection > 0.0 && projection < distanceToLight)
            {
                float3 closestPoint = Position_World + lightDir * projection;
                float distToCenter = distance(occluderCenter, closestPoint);
                
                float currentShadow = smoothstep(radius, radius + softness, distToCenter);
                eclipseShadow *= currentShadow;
            }
        }
        
        // Combine self-shadowing, distance falloff, and eclipse shadows
        accumulatedLight += starColor * (selfShadow * attenuation * eclipseShadow);
    }
    
    FinalLightContribution = accumulatedLight;
}

#endif // CUSTOM_SPACE_SHADOWS_INCLUDED
