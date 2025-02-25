using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CameraFollowGizmo
{
    private static GameObject targetObject;
    private static bool followTarget = false;

    static CameraFollowGizmo()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(45, 10, 250, 100), "Camera Follow Gizmo", GUI.skin.window);
        GUILayout.Label("Select a GameObject and click 'Follow'");

        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button(followTarget ? "Stop Following" : "Follow"))
        {
            followTarget = !followTarget;
        }

        GUILayout.EndArea();
        Handles.EndGUI();

        if (followTarget && targetObject != null)
        {
            if (sceneView != null && sceneView.camera != null)
            {
                sceneView.LookAt(targetObject.transform.position);
                sceneView.Repaint();
            }
        }
    }
}