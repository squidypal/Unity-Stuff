using UnityEngine;

[ExecuteInEditMode]
public class BoundingBoxGizmo : MonoBehaviour
{
    // Customizable fields
    public Color boundingBoxColor = Color.blue;
    public Vector3 boxSize = new Vector3(10, 10, 10);
    public bool showGizmo = true;
    public bool snapToBounds = false;

    // Draws the bounding box in the scene
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = boundingBoxColor;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }

#if UNITY_EDITOR
    // Optionally snap the object to the bounds when selected
    void OnDrawGizmosSelected()
    {
        if (snapToBounds)
        {
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, -boxSize.x / 2, boxSize.x / 2);
            position.y = Mathf.Clamp(position.y, -boxSize.y / 2, boxSize.y / 2);
            position.z = Mathf.Clamp(position.z, -boxSize.z / 2, boxSize.z / 2);
            transform.position = position;
        }
    }
#endif
}