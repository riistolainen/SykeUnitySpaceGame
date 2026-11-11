#ifndef CUSTOM_SPACE_SHADOWS_INCLUDED
#define CUSTOM_SPACE_SHADOWS_INCLUDED

// --- Global Memory Buffers (Passed via C# script) ---
// Support for up to 16 shadow-casting occluders
uniform float4 _OccluderPositions[16];
uniform float _OccluderRadii[16];
uniform float _OccluderSoftness[16];

// Support for up to 16 separate light/star sources
uniform float4 _StarPositions[16]; // xyz = Position, w = Range/Radius of influence
uniform float4 _StarColors[16]; // rgb = Color, a = Base Intensity

void CalculateMultiLightSpaceShading_float(
    float3 Position_World,
    float OccluderCount,
    float LightCount,
    out float3 FinalLightContribution)
{
    // Start with zero accumulated light
    float3 accumulatedLight = float3(0.0, 0.0, 0.0);
    
    int totalLights = min(16, (int) LightCount);
    int totalOccluders = min(16, (int) OccluderCount);
    
    // LOOP 1: Process every light source individually
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
        
        // --- 1. Calculate Distance Attenuation (Inverse Square Falloff with Smooth Window) ---
        float attenuation = 1.0 / max(1.0, distanceToLight * distanceToLight);
        
        // Graceful fade to 0 when nearing the edge of the star's max range
        float rangeFade = smoothstep(starRange, starRange * 0.8, distanceToLight);
        attenuation *= rangeFade * starIntensity;
        
        // Early out if this light source provides no meaningful illumination to the pixel
        if (attenuation <= 0.001)
        {
            continue;
        }
        
        // --- 2. Calculate Shadow Attenuation from Occluders for THIS specific light ---
        float shadowFactor = 1.0;
        
        for (int o = 0; o < totalOccluders; o++)
        {
            float3 occluderCenter = _OccluderPositions[o].xyz;
            float radius = _OccluderRadii[o];
            float softness = _OccluderSoftness[o];
            
            float3 v = occluderCenter - Position_World;
            float projection = dot(v, lightDir);
            
            // Check if occluder sits between the pixel and the star
            if (projection > 0.0 && projection < distanceToLight)
            {
                float3 closestPoint = Position_World + lightDir * projection;
                float distToCenter = distance(occluderCenter, closestPoint);
                
                // Shadow penumbra calculation
                float currentShadow = smoothstep(radius, radius + softness, distToCenter);
                shadowFactor *= currentShadow;
            }
        }
        
        // Accumulate this light's final masked energy color
        accumulatedLight += starColor * (attenuation * shadowFactor);
    }
    
    // Output the combined color array back into the node graph
    FinalLightContribution = accumulatedLight;
}

#endif // CUSTOM_SPACE_SHADOWS_INCLUDED
