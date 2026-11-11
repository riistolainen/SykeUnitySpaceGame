using UnityEngine;

[ExecuteAlways]
public class SpaceOccluder : MonoBehaviour
{
    public float radius = 1.0f;
    public float softness = 0.5f;

    private void OnEnable() => SpaceLightingManager.Instance.RegisterOccluder(this);
    private void OnDisable() => SpaceLightingManager.Instance.UnregisterOccluder(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawWireSphere(transform.position, radius + softness);
    }
}