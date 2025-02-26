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
        public bool enableScreeningEffect = false;
        public float screeningThreshold = 0.3f;
        public float screeningStrength = 0.5f;
        
        [HideInInspector]
        public Color previousColor = Color.black;
    }
    
    public List<LightMapping> lightMappings = new List<LightMapping>();
    public float updateInterval = 0.1f;
    public bool globalScreeningEffect = false;
    [Range(0.1f, 1.0f)]
    public float globalScreeningStrength = 0.5f;
    [Range(0.1f, 1.0f)]
    public float globalScreeningThreshold = 0.3f;
    
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
        
        // Initialize previous colors
        foreach (var mapping in lightMappings)
        {
            if (mapping.targetLight != null)
            {
                mapping.previousColor = mapping.targetLight.color;
            }
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

            bool useScreening = mapping.enableScreeningEffect || globalScreeningEffect;
            if (useScreening)
            {
                float threshold = globalScreeningEffect ? globalScreeningThreshold : mapping.screeningThreshold;
                float strength = globalScreeningEffect ? globalScreeningStrength : mapping.screeningStrength;
                
                sampledColor = ApplyScreeningEffect(sampledColor, mapping.previousColor, threshold, strength);
            }
            
            mapping.targetLight.color = sampledColor;
            mapping.targetLight.enabled = sampledColor.grayscale > 0.01f;

            mapping.previousColor = sampledColor;
        }
    }
    
    Color ApplyScreeningEffect(Color newColor, Color previousColor, float threshold, float strength)
    {
        float prevBrightness = previousColor.grayscale;
        float newBrightness = newColor.grayscale;
        float brightnessDiff = Mathf.Abs(newBrightness - prevBrightness);

        if (brightnessDiff > threshold)
        {
            float blendFactor = Mathf.Min(1.0f, (brightnessDiff - threshold) / (1.0f - threshold)) * strength;
            
            float targetBrightness;
            if (newBrightness > prevBrightness)
            {
                targetBrightness = prevBrightness + (newBrightness - prevBrightness) * (1.0f - blendFactor);
            }
            else
            {
                targetBrightness = prevBrightness - (prevBrightness - newBrightness) * (1.0f - blendFactor);
            }
            
            if (newBrightness > 0.001f)
            {
                float ratio = targetBrightness / newBrightness;
                return new Color(
                    newColor.r * ratio,
                    newColor.g * ratio,
                    newColor.b * ratio,
                    newColor.a
                );
            }
        }
        
        return newColor;
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