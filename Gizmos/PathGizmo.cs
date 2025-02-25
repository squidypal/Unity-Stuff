using UnityEngine;

[ExecuteInEditMode]
public class PathGizmo : MonoBehaviour
{
    // Customizable fields
    public Color pathColor = Color.yellow;
    public Transform[] waypoints;
    public bool showArrows = false;
    public bool showGizmo = true;
    public float waypointSize = 0.3f;
    public float arrowSize = 0.5f;

    // Draws the path gizmo
    void OnDrawGizmos()
    {
        if (showGizmo && waypoints.Length > 1)
        {
            Gizmos.color = pathColor;

            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                // Draw the line between waypoints
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);

                // Optionally draw arrows to indicate direction
                if (showArrows)
                {
                    DrawArrow(waypoints[i].position, waypoints[i + 1].position, arrowSize);
                }
            }

            // Draw waypoint markers
            foreach (Transform waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, waypointSize);
            }
        }
    }

    // Helper function to draw an arrow between points
    void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * Vector3.forward;

        Gizmos.DrawRay(end, right * arrowLength);
        Gizmos.DrawRay(end, left * arrowLength);
    }
}