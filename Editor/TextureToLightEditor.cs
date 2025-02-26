#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureToLight))]
public class TextureToLightEditor : Editor
{
    private TextureToLight script;
    
    void OnEnable()
    {
        script = (TextureToLight)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (script.renderTexture == null)
            return;
            
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Render Texture Preview", EditorStyles.boldLabel);
        
        float width = EditorGUIUtility.currentViewWidth - 30;
        float height = width * script.renderTexture.height / script.renderTexture.width;
        
        Rect previewRect = GUILayoutUtility.GetRect(width, height);
        
        EditorGUI.DrawPreviewTexture(previewRect, script.renderTexture);
        
        Vector2 mousePos = Event.current.mousePosition;
        
        if (Event.current.type == EventType.MouseDown && previewRect.Contains(mousePos))
        {
            Vector2 normalizedPos = new Vector2(
                (mousePos.x - previewRect.x) / previewRect.width,
                1.0f - (mousePos.y - previewRect.y) / previewRect.height
            );
            
            script.samplePosition = normalizedPos;
            EditorUtility.SetDirty(script);
            Repaint();
        }
        
        float dotSize = 10f;
        Rect dotRect = new Rect(
            previewRect.x + previewRect.width * script.samplePosition.x - dotSize/2,
            previewRect.y + previewRect.height * (1.0f - script.samplePosition.y) - dotSize/2,
            dotSize,
            dotSize
        );
        
        EditorGUI.DrawRect(dotRect, script.gizmoColor);
        
        if (script.targetLight != null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Sampled Color:", EditorStyles.boldLabel);
            
            Rect colorRect = GUILayoutUtility.GetRect(width, 20);
            EditorGUI.DrawRect(colorRect, script.targetLight.color);
        }
    }
    
    void OnSceneGUI()
    {
        if (!script.showGizmo || script.renderTexture == null)
            return;
        
        Vector3 gizmoPosition = script.transform.position + Vector3.up * script.gizmoSize * 2;
        
        Handles.color = script.gizmoColor;
        Handles.SphereHandleCap(0, gizmoPosition, Quaternion.identity, script.gizmoSize * 2, EventType.Repaint);
        
        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(gizmoPosition, Quaternion.identity);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "Move Sample Position");
            
            float distanceX = newPosition.x - script.transform.position.x;
            float distanceY = newPosition.y - script.transform.position.y;
            
            script.samplePosition = new Vector2(
                Mathf.Clamp01(0.5f + distanceX),
                Mathf.Clamp01(0.5f + distanceY)
            );
            
            EditorUtility.SetDirty(script);
        }
    }
}
#endif