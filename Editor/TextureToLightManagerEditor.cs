#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureToLightManager))]
public class TextureToLightManagerEditor : Editor
{
    private TextureToLightManager script;
    private int selectedMappingIndex = 0;
    private string[] mappingLabels;
    
    void OnEnable()
    {
        script = (TextureToLightManager)target;
        UpdateMappingLabels();
    }
    
    void UpdateMappingLabels()
    {
        if (script.lightMappings == null || script.lightMappings.Count == 0)
        {
            mappingLabels = new string[] { "No Mappings" };
            return;
        }
        
        mappingLabels = new string[script.lightMappings.Count];
        for (int i = 0; i < script.lightMappings.Count; i++)
        {
            var mapping = script.lightMappings[i];
            string textureName = mapping.renderTexture ? mapping.renderTexture.name : "None";
            string lightName = mapping.targetLight ? mapping.targetLight.name : "None";
            mappingLabels[i] = $"Mapping {i+1}: {textureName} → {lightName}";
        }
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            UpdateMappingLabels();
        }
        
        if (script.lightMappings == null || script.lightMappings.Count == 0)
            return;
            
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Render Texture Preview", EditorStyles.boldLabel);
        
        selectedMappingIndex = EditorGUILayout.Popup("Select Mapping", selectedMappingIndex, mappingLabels);
        
        if (selectedMappingIndex >= script.lightMappings.Count)
        {
            selectedMappingIndex = script.lightMappings.Count - 1;
        }
        
        var selectedMapping = script.lightMappings[selectedMappingIndex];
        
        if (selectedMapping.renderTexture == null)
        {
            EditorGUILayout.HelpBox("No render texture assigned to this mapping.", MessageType.Info);
            return;
        }
        
        float width = EditorGUIUtility.currentViewWidth - 30;
        float height = width * selectedMapping.renderTexture.height / selectedMapping.renderTexture.width;
        
        Rect previewRect = GUILayoutUtility.GetRect(width, height);
        
        EditorGUI.DrawPreviewTexture(previewRect, selectedMapping.renderTexture);
        
        Vector2 mousePos = Event.current.mousePosition;
        
        if (Event.current.type == EventType.MouseDown && previewRect.Contains(mousePos))
        {
            Vector2 normalizedPos = new Vector2(
                (mousePos.x - previewRect.x) / previewRect.width,
                1.0f - (mousePos.y - previewRect.y) / previewRect.height
            );
            
            selectedMapping.samplePosition = normalizedPos;
            EditorUtility.SetDirty(script);
            Repaint();
        }
        
        // Draw the sample point
        float dotSize = 10f;
        Rect dotRect = new Rect(
            previewRect.x + previewRect.width * selectedMapping.samplePosition.x - dotSize/2,
            previewRect.y + previewRect.height * (1.0f - selectedMapping.samplePosition.y) - dotSize/2,
            dotSize,
            dotSize
        );
        
        Color dotColor = selectedMapping.targetLight ? selectedMapping.targetLight.color : Color.red;
        EditorGUI.DrawRect(dotRect, dotColor);

        bool useAreaAverage = selectedMapping.enableAreaAverage || script.globalAreaAverage;
        if (useAreaAverage)
        {
            int radius = script.globalAreaAverage ? script.globalSampleRadius : selectedMapping.sampleRadius;
            
            float pixelSize = previewRect.width / selectedMapping.renderTexture.width;
            float radiusSize = radius * pixelSize;

            Handles.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0.3f);
            Vector3 center = new Vector3(
                previewRect.x + previewRect.width * selectedMapping.samplePosition.x,
                previewRect.y + previewRect.height * (1.0f - selectedMapping.samplePosition.y),
                0
            );
            Handles.DrawWireDisc(center, Vector3.forward, radiusSize);
            
            GUI.color = Color.white;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Sample Radius: {radius} pixels", EditorStyles.boldLabel);
        }
        
        if (selectedMapping.targetLight != null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Sampled Color:", EditorStyles.boldLabel);
            
            Rect colorRect = GUILayoutUtility.GetRect(width, 20);
            EditorGUI.DrawRect(colorRect, selectedMapping.targetLight.color);
        }
    }
}
#endif