using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public bool enableSmoothTransition = false;
        public float transitionSpeed = 5.0f;
        public bool enableAreaAverage = false;
        [Range(1, 1000)]
        public int sampleRadius = 5;
        
        [HideInInspector]
        public Color previousColor = Color.black;
        [HideInInspector]
        public Color targetColor = Color.black;
        [HideInInspector]
        public float transitionProgress = 1.0f;
    }
    
    public List<LightMapping> lightMappings = new List<LightMapping>();
    public float updateInterval = 0.1f;
    public bool globalScreeningEffect = false;
    [Range(0.1f, 1.0f)]
    public float globalScreeningStrength = 0.5f;
    [Range(0.1f, 1.0f)]
    public float globalScreeningThreshold = 0.3f;
    public bool globalSmoothTransition = false;
    [Range(1.0f, 10.0f)]
    public float globalTransitionSpeed = 5.0f;
    public bool globalAreaAverage = false;
    [Range(1, 1000)]
    public int globalSampleRadius = 5;
    public int batchSize = 3;
    
    private Texture2D tempTexture;
    private RenderTexture tempRT;
    private float timeSinceLastUpdate = 0f;
    private int currentBatchIndex = 0;
    
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
        
        foreach (var mapping in lightMappings)
        {
            if (mapping.targetLight != null)
            {
                mapping.previousColor = mapping.targetLight.color;
                mapping.targetColor = mapping.targetLight.color;
            }
        }
    }
    
    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;
        
        if (timeSinceLastUpdate >= updateInterval)
        {
            ProcessLightsBatch();
            timeSinceLastUpdate = 0f;
        }

        UpdateSmoothTransitions();
    }
    
    void ProcessLightsBatch()
    {
        if (lightMappings.Count == 0) return;
        
        int totalMappings = lightMappings.Count;
        int actualBatchSize = Mathf.Min(batchSize, totalMappings);
        
        Dictionary<RenderTexture, Dictionary<Vector2, Color>> rtSampleCache = new Dictionary<RenderTexture, Dictionary<Vector2, Color>>();
        Dictionary<RenderTexture, Dictionary<Vector2, Dictionary<int, Color>>> rtAreaSampleCache = new Dictionary<RenderTexture, Dictionary<Vector2, Dictionary<int, Color>>>();
        
        for (int i = 0; i < actualBatchSize; i++)
        {
            int mappingIndex = (currentBatchIndex + i) % totalMappings;
            var mapping = lightMappings[mappingIndex];
            
            if (mapping.renderTexture == null || mapping.targetLight == null)
                continue;
                
            bool useAreaAverage = mapping.enableAreaAverage || globalAreaAverage;
            int sampleRadius = globalAreaAverage ? globalSampleRadius : mapping.sampleRadius;
            
            Color sampledColor;
            
            if (useAreaAverage)
            {
                if (!rtAreaSampleCache.ContainsKey(mapping.renderTexture))
                {
                    rtAreaSampleCache[mapping.renderTexture] = new Dictionary<Vector2, Dictionary<int, Color>>();
                }
                
                if (!rtAreaSampleCache[mapping.renderTexture].ContainsKey(mapping.samplePosition))
                {
                    rtAreaSampleCache[mapping.renderTexture][mapping.samplePosition] = new Dictionary<int, Color>();
                }
                
                if (!rtAreaSampleCache[mapping.renderTexture][mapping.samplePosition].ContainsKey(sampleRadius))
                {
                    rtAreaSampleCache[mapping.renderTexture][mapping.samplePosition][sampleRadius] = 
                        SampleRenderTexture(mapping.renderTexture, mapping.samplePosition, true, sampleRadius);
                }
                
                sampledColor = rtAreaSampleCache[mapping.renderTexture][mapping.samplePosition][sampleRadius];
            }
            else
            {
                if (!rtSampleCache.ContainsKey(mapping.renderTexture))
                {
                    rtSampleCache[mapping.renderTexture] = new Dictionary<Vector2, Color>();
                }
                
                if (!rtSampleCache[mapping.renderTexture].ContainsKey(mapping.samplePosition))
                {
                    rtSampleCache[mapping.renderTexture][mapping.samplePosition] = 
                        SampleRenderTexture(mapping.renderTexture, mapping.samplePosition, false);
                }
                
                sampledColor = rtSampleCache[mapping.renderTexture][mapping.samplePosition];
            }

            bool useScreening = mapping.enableScreeningEffect || globalScreeningEffect;
            if (useScreening)
            {
                float threshold = globalScreeningEffect ? globalScreeningThreshold : mapping.screeningThreshold;
                float strength = globalScreeningEffect ? globalScreeningStrength : mapping.screeningStrength;
                
                sampledColor = ApplyScreeningEffect(sampledColor, mapping.previousColor, threshold, strength);
            }
            
            bool useSmoothTransition = mapping.enableSmoothTransition || globalSmoothTransition;
            if (useSmoothTransition)
            {
                mapping.targetColor = sampledColor;
                mapping.transitionProgress = 0.0f;
            }
            else
            {
                mapping.targetLight.color = sampledColor;
                mapping.previousColor = sampledColor;
            }
        }
        
        currentBatchIndex = (currentBatchIndex + actualBatchSize) % totalMappings;
    }
    
    void UpdateSmoothTransitions()
    {
        foreach (var mapping in lightMappings)
        {
            bool useSmoothTransition = mapping.enableSmoothTransition || globalSmoothTransition;
            if (useSmoothTransition && mapping.transitionProgress < 1.0f && mapping.targetLight != null)
            {
                float speed = globalSmoothTransition ? globalTransitionSpeed : mapping.transitionSpeed;
                mapping.transitionProgress += Time.deltaTime * speed;
                if (mapping.transitionProgress > 1.0f)
                    mapping.transitionProgress = 1.0f;
                Color currentColor = Color.Lerp(mapping.previousColor, mapping.targetColor, mapping.transitionProgress);
                mapping.targetLight.color = currentColor;
                if (mapping.transitionProgress >= 1.0f)
                {
                    mapping.previousColor = mapping.targetColor;
                }
            }
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
    
    Color SampleRenderTexture(RenderTexture rt, Vector2 normalizedPosition, bool useAreaAverage = false, int radius = 1)
    {
        int centerX = Mathf.FloorToInt(normalizedPosition.x * rt.width);
        int centerY = Mathf.FloorToInt(normalizedPosition.y * rt.height);
        
        if (!useAreaAverage || radius <= 1)
        {
            tempRT.DiscardContents();
            Graphics.CopyTexture(rt, 0, 0, centerX, centerY, 1, 1, tempRT, 0, 0, 0, 0);
            
            RenderTexture.active = tempRT;
            tempTexture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
            tempTexture.Apply();
            
            Color sampledColor = tempTexture.GetPixel(0, 0);
            RenderTexture.active = null;
            
            return sampledColor;
        }
        else
        {
            int size = radius * 2 + 1;
            int startX = Mathf.Max(0, centerX - radius);
            int startY = Mathf.Max(0, centerY - radius);
            int width = Mathf.Min(size, rt.width - startX);
            int height = Mathf.Min(size, rt.height - startY);
            
            if (tempRT.width < width || tempRT.height < height)
            {
                tempRT.Release();
                tempRT = new RenderTexture(width, height, 0, rt.format);
            }
            
            tempRT.DiscardContents();
            Graphics.CopyTexture(rt, 0, 0, startX, startY, width, height, tempRT, 0, 0, 0, 0);
            
            RenderTexture.active = tempRT;
            
            if (tempTexture.width != width || tempTexture.height != height)
            {
                Destroy(tempTexture);
                tempTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            }
            
            tempTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tempTexture.Apply();
            
            Color averageColor = Color.black;
            int sampleCount = 0;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float distX = x - (centerX - startX);
                    float distY = y - (centerY - startY);
                    float distSqr = distX * distX + distY * distY;
                    
                    if (distSqr <= radius * radius)
                    {
                        Color pixelColor = tempTexture.GetPixel(x, y);
                        averageColor += pixelColor;
                        sampleCount++;
                    }
                }
            }
            
            RenderTexture.active = null;
            
            if (sampleCount > 0)
            {
                averageColor /= sampleCount;
            }
            
            return averageColor;
        }
    }
    
    void OnDestroy()
    {
        if (tempTexture != null)
            Destroy(tempTexture);
        
        if (tempRT != null)
            tempRT.Release();
    }
}