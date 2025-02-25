using UnityEngine;

[ExecuteInEditMode]
public class PatrolRouteGizmo : MonoBehaviour
{
    // Customizable fields
    public Color waypointColor = Color.green;
    public Color routeColor = Color.yellow;
    public float waypointSize = 0.5f;
    public bool showWaypoints = true;
    public bool showRoute = true;

    public Transform[] waypoints;  // Assign waypoints in the inspector

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        if (showWaypoints)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                // Draw waypoint markers
                Gizmos.color = waypointColor;
                Gizmos.DrawSphere(waypoints[i].position, waypointSize);

                // Draw waypoint index label
                Gizmos.color = Color.white;
                Gizmos.DrawIcon(waypoints[i].position, i.ToString(), true);
            }
        }

        if (showRoute)
        {
            // Draw lines connecting waypoints to form the patrol route
            Gizmos.color = routeColor;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}