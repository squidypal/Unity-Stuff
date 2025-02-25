using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MovementRotationBoundaryGizmo : MonoBehaviour
{
    // Customizable fields
    public Color movementBoundaryColor = Color.blue;
    public Color rotationBoundaryColor = Color.green;
    public Vector3 movementBoundarySize = new Vector3(10, 1, 10);
    public float rotationLimit = 45f; // in degrees
    public bool showMovementBoundary = true;
    public bool showRotationBoundary = true;

    // Draws the boundaries using gizmos
    void OnDrawGizmos()
    {
        if (showMovementBoundary)
        {
            Gizmos.color = movementBoundaryColor;
            Gizmos.DrawWireCube(transform.position, movementBoundarySize);
        }

        if (showRotationBoundary)
        {
            Gizmos.color = rotationBoundaryColor;
            Vector3 rotationStart = transform.position + Vector3.up * 2.0f;
            Quaternion rotationLeft = Quaternion.Euler(0, -rotationLimit, 0);
            Quaternion rotationRight = Quaternion.Euler(0, rotationLimit, 0);

            // Draw rotation arc lines
            Gizmos.DrawRay(rotationStart, rotationLeft * Vector3.forward * 5.0f);
            Gizmos.DrawRay(rotationStart, rotationRight * Vector3.forward * 5.0f);
        }
    }

    // Interactive handles to resize movement boundaries
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (showMovementBoundary)
        {
            movementBoundarySize = Handles.ScaleHandle(movementBoundarySize, transform.position, Quaternion.identity, 1.0f);
        }
    }
#endif
}