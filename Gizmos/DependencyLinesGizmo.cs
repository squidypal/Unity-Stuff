using UnityEngine;

[ExecuteInEditMode]
public class DependencyLinesGizmo : MonoBehaviour
{
    // Customizable fields
    public Color lineColor = Color.yellow;
    public bool showGizmo = true;
    public Transform[] dependentObjects;  // Assign dependent objects in the Inspector

    // Draws lines showing object dependencies
    void OnDrawGizmos()
    {
        if (!showGizmo || dependentObjects == null || dependentObjects.Length == 0)
            return;

        Gizmos.color = lineColor;

        foreach (Transform dependentObject in dependentObjects)
        {
            if (dependentObject != null)
            {
                // Draw line from this object to its dependent object
                Gizmos.DrawLine(transform.position, dependentObject.position);
                Gizmos.DrawIcon(dependentObject.position, "Dependency", true);
            }
        }
    }

    // Optionally visualize parent-child relationships
    void OnDrawGizmosSelected()
    {
        if (transform.parent != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.parent.position);
            Gizmos.DrawIcon(transform.parent.position, "Parent", true);
        }
    }
}