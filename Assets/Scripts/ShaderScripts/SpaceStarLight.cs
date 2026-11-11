using UnityEngine;

[ExecuteAlways]
public class SpaceStarLight : MonoBehaviour
{
    [ColorUsage(false, true)] public Color starColor = Color.white;
    public float intensity = 1.0f;
    public float lightRange = 100.0f;

    private void OnEnable() => SpaceLightingManager.Instance.RegisterStar(this);
    private void OnDisable() => SpaceLightingManager.Instance.UnregisterStar(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = starColor;
        Gizmos.DrawWireSphere(transform.position, lightRange);
    }
}
