using UnityEngine;

[ExecuteAlways]
public class SpaceStarLight : MonoBehaviour
{
    [ColorUsage(false, true)] public Color starColor = new Color(1024f, 1024f, 1024f, 1f);


    public float intensity = 1000f;
    public float lightRange = 10000f;

    private void OnEnable() => SpaceLightingManager.Instance?.RegisterStar(this);
    private void OnDisable() => SpaceLightingManager.Instance?.UnregisterStar(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = starColor;
        Gizmos.DrawWireSphere(transform.position, lightRange);
    }
}
