using UnityEngine;

[ExecuteInEditMode]
public class ZoneGizmo : MonoBehaviour
{
    // Customizable fields
    public Color zoneColor = new Color(0, 0, 1, 0.25f); // Semi-transparent blue
    public Vector3 zoneSize = new Vector3(5, 5, 5);
    public bool isWireframe = true;
    public bool showGizmo = true;

    // Draws the zone gizmo
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = zoneColor;
            if (isWireframe)
            {
                Gizmos.DrawWireCube(transform.position, zoneSize);
            }
            else
            {
                Gizmos.DrawCube(transform.position, zoneSize);
            }
        }
    }

    // Draws only when selected
    void OnDrawGizmosSelected()
    {
        if (!showGizmo)
        {
            Gizmos.color = zoneColor;
            if (isWireframe)
            {
                Gizmos.DrawWireCube(transform.position, zoneSize);
            }
            else
            {
                Gizmos.DrawCube(transform.position, zoneSize);
            }
        }
    }
}