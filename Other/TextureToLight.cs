using UnityEngine;

public class TextureToLight : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Light targetLight;
    public Vector2 samplePosition = new Vector2(0.5f, 0.5f);

    [Header("Gizmo Settings")] public bool showGizmo = true;
    public Color gizmoColor = Color.red;
    public float gizmoSize = 0.05f;

    private Texture2D tempTexture;
    private RenderTexture tempRT;

    void Start()
    {
        tempTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        tempRT = new RenderTexture(1, 1, 0, renderTexture.format);
    }

    Color SampleRenderTexture(RenderTexture rt, Vector2 normalizedPosition)
    {
        int x = Mathf.FloorToInt(normalizedPosition.x * rt.width);
        int y = Mathf.FloorToInt(normalizedPosition.y * rt.height);

        tempRT.DiscardContents();
        Graphics.CopyTexture(rt, 0, 0, x, y, 1, 1, tempRT, 0, 0, 0, 0);

        RenderTexture.active = tempRT;
        tempTexture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        tempTexture.Apply();

        Color sampledColor = tempTexture.GetPixel(0, 0);
        RenderTexture.active = null;

        return sampledColor;
    }

    void OnDestroy()
    {
        // Clean up resources
        if (tempTexture != null)
            Destroy(tempTexture);

        if (tempRT != null)
            tempRT.Release();
    }

    public float updateInterval = 0.1f;
    private float timeSinceLastUpdate = 0f;

    void Update()
    {
        if (renderTexture == null || targetLight == null)
            return;

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            Color sampledColor = SampleRenderTexture(renderTexture, samplePosition);
            targetLight.color = sampledColor;
           // targetLight.enabled = sampledColor.grayscale > 0.01f;

            timeSinceLastUpdate = 0f;
        }

        Color SampleRenderTexture(RenderTexture rt, Vector2 normalizedPosition)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(rt.width, rt.height, 0, rt.format);
            Graphics.Blit(rt, tempRT);
            RenderTexture.active = tempRT;

            int x = Mathf.FloorToInt(normalizedPosition.x * rt.width);
            int y = Mathf.FloorToInt(normalizedPosition.y * rt.height);

            tempTexture.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
            tempTexture.Apply();

            Color sampledColor = tempTexture.GetPixel(0, 0);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(tempRT);

            return sampledColor;
        }

        void OnDrawGizmos()
        {
            if (!showGizmo || renderTexture == null)
                return;

            Color originalColor = Gizmos.color;
            Gizmos.color = gizmoColor;

            Vector3 gizmoPosition = transform.position + Vector3.up * gizmoSize * 2;
            Gizmos.DrawSphere(gizmoPosition, gizmoSize);

            Gizmos.DrawLine(transform.position, gizmoPosition);

            Gizmos.color = originalColor;
        }
    }
}

