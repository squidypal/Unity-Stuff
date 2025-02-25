using UnityEngine;

[ExecuteInEditMode]
public class SpawnProbabilityGizmo : MonoBehaviour
{
    // Customizable fields
    public Color lowProbabilityColor = Color.blue;
    public Color highProbabilityColor = Color.red;
    public float gizmoSize = 1.0f;
    public bool showGizmo = true;

    [Range(0f, 1f)]
    public float spawnProbability = 0.5f;  // Probability value between 0 and 1

    // Draw the heatmap gizmo
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            // Interpolate color based on probability (0 = low, 1 = high)
            Color gizmoColor = Color.Lerp(lowProbabilityColor, highProbabilityColor, spawnProbability);
            Gizmos.color = gizmoColor;

            // Draw sphere representing the probability at the current position
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }
}