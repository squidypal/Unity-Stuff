using System.Collections.Generic;
using UnityEngine;

public class TextureToLightManager : MonoBehaviour
{
    [System.Serializable]
    public class LightMapping
    {
        public RenderTexture renderTexture;
        public Light targetLight;
        public Vector2 samplePosition = new Vector2(0.5f, 0.5f);
    }
    
    public List<LightMapping> lightMappings = new List<LightMapping>();
    public float updateInterval = 0.1f;
    
    private Texture2D tempTexture;
    private RenderTexture tempRT;
    private float timeSinceLastUpdate = 0f;
    
    void Start()
    {
        tempTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        if (lightMappings.Count > 0 && lightMappings[0].renderTexture != null)
        {
            tempRT = new RenderTexture(1, 1, 0, lightMappings[0].renderTexture.format);
        }
        else
        {
            tempRT = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
        }
    }
    
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        
        if (timeSinceLastUpdate >= updateInterval)
        {
            ProcessAllLights();
            timeSinceLastUpdate = 0f;
        }
    }
    
    void ProcessAllLights()
    {
        foreach (var mapping in lightMappings)
        {
            if (mapping.renderTexture == null || mapping.targetLight == null)
                continue;
                
            Color sampledColor = SampleRenderTexture(mapping.renderTexture, mapping.samplePosition);
            mapping.targetLight.color = sampledColor;
            mapping.targetLight.enabled = sampledColor.grayscale > 0.01f;
        }
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
        if (tempTexture != null)
            Destroy(tempTexture);
        
        if (tempRT != null)
            tempRT.Release();
    }
}