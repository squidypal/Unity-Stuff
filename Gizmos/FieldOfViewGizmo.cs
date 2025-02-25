using UnityEngine;

[ExecuteInEditMode]
public class FieldOfViewGizmo : MonoBehaviour
{
    // Customizable fields
    public Color fovColor = Color.green;
    public float viewAngle = 90f;
    public float viewDistance = 10f;
    public bool showGizmo = true;

    // Draws the field of view
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = fovColor;

            Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

            // Draw FOV lines
            Gizmos.DrawRay(transform.position, leftBoundary * viewDistance);
            Gizmos.DrawRay(transform.position, rightBoundary * viewDistance);

            // Draw a circle to visualize the maximum distance
            Gizmos.DrawWireSphere(transform.position, viewDistance);
        }
    }
}