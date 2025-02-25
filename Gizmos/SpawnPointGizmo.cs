using UnityEngine;

[ExecuteInEditMode]
public class SpawnPointGizmo : MonoBehaviour
{
    // Customizable fields
    public Color spawnPointColor = Color.green;
    public Color directionColor = Color.red;
    public float markerSize = 0.5f;
    public bool showGizmo = true;

    // Draws Gizmos only when selected or always depending on settings
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            // Draw spawn point marker
            Gizmos.color = spawnPointColor;
            Gizmos.DrawSphere(transform.position, markerSize);

            // Draw forward direction arrow
            Gizmos.color = directionColor;
            Gizmos.DrawRay(transform.position, transform.forward * 2.0f);
        }
    }

    // Adding some convenience to make gizmo behavior consistent when selected
    void OnDrawGizmosSelected()
    {
        if (!showGizmo)
        {
            // Draw spawn point marker
            Gizmos.color = spawnPointColor;
            Gizmos.DrawSphere(transform.position, markerSize);

            // Draw forward direction arrow
            Gizmos.color = directionColor;
            Gizmos.DrawRay(transform.position, transform.forward * 2.0f);
        }
    }
}