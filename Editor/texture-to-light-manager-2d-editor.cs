#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

[CustomEditor(typeof(TextureToLightManager2D))]
public class TextureToLightManager2DEditor : Editor
{
    private TextureToLightManager2D script;
    private int selectedMappingIndex = 0;
    private string[] mappingLabels;
    
    void OnEnable()
    {
        script = (TextureToLightManager2D)target;
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
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("updateInterval"));
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("globalScreeningEffect"));
        if (script.globalScreeningEffect)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalScreeningThreshold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalScreeningStrength"));
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("globalSmoothTransition"));
        if (script.globalSmoothTransition)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalTransitionSpeed"));
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("globalAreaAverage"));
        if (script.globalAreaAverage)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalSampleRadius"));
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Light Mappings", EditorStyles.boldLabel);
        
        SerializedProperty mappingsProperty = serializedObject.FindProperty("lightMappings");
        
        if (GUILayout.Button("Add New Mapping"))
        {
            mappingsProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
            UpdateMappingLabels();
        }
        
        for (int i = 0; i < mappingsProperty.arraySize; i++)
        {
            SerializedProperty mappingProperty = mappingsProperty.GetArrayElementAtIndex(i);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            string title = i < mappingLabels.Length ? mappingLabels[i] : $"Mapping {i+1}";
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                mappingsProperty.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                UpdateMappingLabels();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("renderTexture"));
            
            EditorGUILayout.BeginHorizontal();
            SerializedProperty targetLightProp = mappingProperty.FindPropertyRelative("targetLight");
            EditorGUILayout.PropertyField(targetLightProp, new GUIContent("Target Light"));
            
            if (GUILayout.Button("Pick", GUILayout.Width(50)))
            {
                GameObject pickedObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
                if (pickedObject != null)
                {
                    Light2D light = pickedObject.GetComponent<Light2D>();
                    if (light != null)
                    {
                        targetLightProp.objectReferenceValue = light;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("samplePosition"));
            
            EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("enableScreeningEffect"));
            if (mappingProperty.FindPropertyRelative("enableScreeningEffect").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("screeningThreshold"));
                EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("screeningStrength"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("enableSmoothTransition"));
            if (mappingProperty.FindPropertyRelative("enableSmoothTransition").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("transitionSpeed"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("enableAreaAverage"));
            if (mappingProperty.FindPropertyRelative("enableAreaAverage").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(mappingProperty.FindPropertyRelative("sampleRadius"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        serializedObject.ApplyModifiedProperties();
        
        if (GUI.changed)
        {
            UpdateMappingLabels();
        }
        
        if (script.lightMappings != null && script.lightMappings.Count > 0)
        {
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
                
                EditorGUILayout.LabelField($"Light Intensity: {selectedMapping.targetLight.intensity.ToString("F2")}", EditorStyles.boldLabel);
            }
        }
    }
}
#endif