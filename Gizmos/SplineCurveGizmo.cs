using UnityEngine;

[ExecuteInEditMode]
public class SplineCurveGizmo : MonoBehaviour
{
    // Customizable fields
    public Color splineColor = Color.cyan;
    public float thickness = 0.1f;
    public bool showGizmo = true;
    public Transform[] controlPoints;  // Assign control points in the Inspector
    [Range(2, 50)]
    public int curveResolution = 20;  // Controls how smooth the spline is

    // Draws the spline curve in the scene
    void OnDrawGizmos()
    {
        if (!showGizmo || controlPoints == null || controlPoints.Length < 2)
            return;

        Gizmos.color = splineColor;

        Vector3 previousPoint = controlPoints[0].position;

        // Draw curve between control points using a simple Catmull-Rom spline
        for (int i = 1; i < controlPoints.Length - 2; i++)
        {
            for (int j = 0; j <= curveResolution; j++)
            {
                float t = j / (float)curveResolution;
                Vector3 point = CalculateCatmullRomSpline(
                    controlPoints[i - 1].position,
                    controlPoints[i].position,
                    controlPoints[i + 1].position,
                    controlPoints[i + 2].position,
                    t);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }

    // Catmull-Rom spline calculation
    private Vector3 CalculateCatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float a = -0.5f * t3 + t2 - 0.5f * t;
        float b = 1.5f * t3 - 2.5f * t2 + 1.0f;
        float c = -1.5f * t3 + 2.0f * t2 + 0.5f * t;
        float d = 0.5f * t3 - 0.5f * t2;

        return a * p0 + b * p1 + c * p2 + d * p3;
    }
}