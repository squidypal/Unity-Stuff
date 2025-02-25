using UnityEngine;
using UnityEditor;  // <-- Ensure this is included for Editor scripting

public class MassMaterialApplier : EditorWindow
{
    private Material materialToApply;

    [MenuItem("Tools/Mass Material Applier")]
    public static void ShowWindow()
    {
        // Show the editor window
        GetWindow<MassMaterialApplier>("Mass Material Applier");
    }

    void OnGUI()
    {
        GUILayout.Label("Mass Material Applier", EditorStyles.boldLabel);

        // Field for selecting the material to apply
        materialToApply = (Material)EditorGUILayout.ObjectField("Material to Apply", materialToApply, typeof(Material), false);

        // Button to apply the material
        if (GUILayout.Button("Apply Material to Selected Objects"))
        {
            ApplyMaterialToSelectedObjects();
        }
    }

    private void ApplyMaterialToSelectedObjects()
    {
        // Get all selected objects
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected to apply material.");
            return;
        }

        if (materialToApply == null)
        {
            Debug.LogWarning("No material selected to apply.");
            return;
        }

        Undo.RecordObjects(selectedObjects, "Mass Material Apply");

        foreach (GameObject obj in selectedObjects)
        {
            // Get the Renderer component
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Apply the material to the object
                renderer.sharedMaterial = materialToApply;
            }
            else
            {
                Debug.LogWarning("Object " + obj.name + " does not have a Renderer component.");
            }
        }

        Debug.Log("Applied material to " + selectedObjects.Length + " objects.");
    }
}