using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class GridGizmo : MonoBehaviour
{
    // Customizable fields
    public Color gridColor = Color.gray;
    public int gridSize = 10;
    public float cellSize = 1f;
    public bool showGrid = true;
    public bool snapToGrid = false;

    void OnDrawGizmos()
    {
        if (showGrid)
        {
            Gizmos.color = gridColor;

            for (int x = -gridSize; x <= gridSize; x++)
            {
                for (int z = -gridSize; z <= gridSize; z++)
                {
                    Vector3 start = new Vector3(x * cellSize, 0, -gridSize * cellSize);
                    Vector3 end = new Vector3(x * cellSize, 0, gridSize * cellSize);
                    Gizmos.DrawLine(start, end);

                    start = new Vector3(-gridSize * cellSize, 0, z * cellSize);
                    end = new Vector3(gridSize * cellSize, 0, z * cellSize);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }

#if UNITY_EDITOR
    // Snap objects to grid when moved in the editor
    void OnDrawGizmosSelected()
    {
        if (snapToGrid)
        {
            Vector3 position = transform.position;
            position.x = Mathf.Round(position.x / cellSize) * cellSize;
            position.z = Mathf.Round(position.z / cellSize) * cellSize;
            transform.position = position;
        }
    }
#endif
}