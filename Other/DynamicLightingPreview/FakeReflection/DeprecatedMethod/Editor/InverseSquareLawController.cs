using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class InverseSquareLawController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float centerX = 0.5f;
    [Range(0f, 1f)]
    public float centerY = 0.5f;
    
    [Range(0.1f, 5f)]
    public float intensity = 1.0f;
    
    [Range(0f, 0.5f)]
    public float falloffStart = 0.1f;
    [Range(0.1f, 1f)]
    public float falloffEnd = 0.5f;
    
    [Range(0.5f, 3f)]
    public float inverseExponent = 2.0f;
    
    [Range(0f, 1f)]
    public float transparencyThreshold = 0.1f;
    
    private RawImage rawImage;
    private Material material;
    
    private int centerPropID;
    private int intensityPropID;
    private int falloffStartPropID;
    private int falloffEndPropID;
    private int inverseExponentPropID;
    private int transparencyThresholdPropID;
    
    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        material = new Material(Shader.Find("UI/InverseSquareLaw"));
        rawImage.material = material;
        
        centerPropID = Shader.PropertyToID("_Center");
        intensityPropID = Shader.PropertyToID("_Intensity");
        falloffStartPropID = Shader.PropertyToID("_FalloffStart");
        falloffEndPropID = Shader.PropertyToID("_FalloffEnd");
        inverseExponentPropID = Shader.PropertyToID("_InverseExponent");
        transparencyThresholdPropID = Shader.PropertyToID("_TransparencyThreshold");
    }
    
    private void Update()
    {
        UpdateShaderProperties();
    }
    
    public void UpdateShaderProperties()
    {
        material.SetVector(centerPropID, new Vector4(centerX, centerY, 0, 0));
        material.SetFloat(intensityPropID, intensity);
        material.SetFloat(falloffStartPropID, falloffStart);
        material.SetFloat(falloffEndPropID, falloffEnd);
        material.SetFloat(inverseExponentPropID, inverseExponent);
        material.SetFloat(transparencyThresholdPropID, transparencyThreshold);
    }
    
    public void SetCenter(Vector2 center)
    {
        centerX = Mathf.Clamp01(center.x);
        centerY = Mathf.Clamp01(center.y);
        UpdateShaderProperties();
    }
    
    public void SetIntensity(float value)
    {
        intensity = Mathf.Max(0.1f, value);
        UpdateShaderProperties();
    }
    
    public void SetFalloffRange(float start, float end)
    {
        falloffStart = Mathf.Clamp(start, 0f, 0.5f);
        falloffEnd = Mathf.Clamp(end, falloffStart + 0.1f, 1f);
        UpdateShaderProperties();
    }
    
    public void SetInverseExponent(float exponent)
    {
        inverseExponent = Mathf.Clamp(exponent, 0.5f, 3f);
        UpdateShaderProperties();
    }
    
    public void SetTransparencyThreshold(float threshold)
    {
        transparencyThreshold = Mathf.Clamp01(threshold);
        UpdateShaderProperties();
    }
}