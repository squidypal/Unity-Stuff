using UnityEngine;

[ExecuteInEditMode]
public class CameraFrustumGizmo : MonoBehaviour
{
    // Customizable fields
    public Color frustumColor = Color.cyan;
    public bool showGizmo = true;

    private Camera cameraComponent;

    void OnDrawGizmos()
    {
        if (!cameraComponent)
        {
            cameraComponent = GetComponent<Camera>();
        }

        if (showGizmo && cameraComponent != null)
        {
            Gizmos.color = frustumColor;

            // Use the camera's position and matrix to draw the frustum
            Gizmos.matrix = cameraComponent.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, cameraComponent.fieldOfView, 
                cameraComponent.farClipPlane, cameraComponent.nearClipPlane, 
                cameraComponent.aspect);
        }
    }
}