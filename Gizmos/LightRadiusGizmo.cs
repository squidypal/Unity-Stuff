using UnityEngine;

[ExecuteInEditMode]
public class LightRadiusGizmo : MonoBehaviour
{
    // Customizable fields
    public Color lightRadiusColor = Color.yellow;
    public bool showGizmo = true;

    private Light lightComponent;

    void OnDrawGizmos()
    {
        if (!lightComponent)
        {
            lightComponent = GetComponent<Light>();
        }

        if (showGizmo && lightComponent != null)
        {
            Gizmos.color = lightRadiusColor;

            if (lightComponent.type == LightType.Point)
            {
                // For point lights, draw a sphere representing the range
                Gizmos.DrawWireSphere(transform.position, lightComponent.range);
            }
            else if (lightComponent.type == LightType.Spot)
            {
                // For spotlights, draw a frustum representing the cone of light
                Gizmos.DrawFrustum(transform.position, lightComponent.spotAngle, 
                    lightComponent.range, 0.1f, 1f);
            }
        }
    }
}