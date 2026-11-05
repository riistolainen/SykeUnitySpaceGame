using UnityEngine;

public class CustomShadowDriver : MonoBehaviour
{
    public Material groundMaterial;
    public Transform virtualLightSource;
    public float shadowRadius = 0.5f;
    public float shadowSoftness = 0.3f;

    void Update()
    {
        if (groundMaterial != null && virtualLightSource != null)
        {
            // Update the math parameters inside the shader every frame
            groundMaterial.SetVector("_Position_Light_1", virtualLightSource.position);
            groundMaterial.SetVector("_Position_Occluder_1", transform.position);
            groundMaterial.SetFloat("_Shadow_1_Softness", shadowRadius);
            groundMaterial.SetFloat("_Shadow_1_Radius", shadowSoftness);
        }
    }
}
