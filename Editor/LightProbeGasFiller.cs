using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class LightProbeFiller
{
    static float probeSpacing = 2f;
    static List<PendingOperation> operationQueue = new();
    static bool isProcessing;
    
    static List<Vector3> recentPositions = new();
    static int maxRecentPositions = 20;

    static ushort[][][] occupancyGrid = new ushort[200][][];

    private static int gridSize = 800;
  
    static float surfaceOffset = -0.25f;
    static RaycastHit hitInfo;

    static LightProbeFiller()
    {
        occupancyGrid = new ushort[gridSize][][];
        EditorApplication.update += ProcessQueue;
    }
    
    static void ProcessQueue()
    {
        int operationsPerFrame = 300;
        for (int i = 0; i < operationsPerFrame; i++) 
        {
            if (operationQueue.Count > 0 && isProcessing)
            {
                PendingOperation currentOp = operationQueue[0];
                operationQueue.RemoveAt(0);
                ProcessRaycast(
                    currentOp.startPoint, 
                    currentOp.rayDirection, 
                    currentOp.mask, 
                    currentOp.rayLength, 
                    currentOp.probeGroup, 
                    currentOp.recursionLevel
                );
            } 
        }
    }
    
    [MenuItem("CONTEXT/LightProbeGroup/Begin Fill")]
    static void StartFill(MenuCommand command)
    {
        LightProbeGroup targetGroup = command.context as LightProbeGroup;
        Vector3 startPosition = targetGroup.transform.position;
        targetGroup.probePositions = new Vector3[0];
        occupancyGrid = new ushort[gridSize][][];
        
        isProcessing = true;
        
        CastInAllDirections(startPosition, LayerMask.GetMask("Default"), probeSpacing, targetGroup, 0);
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("CONTEXT/LightProbeGroup/Stop Fill")]
    static void AbortFill(MenuCommand command)
    {
        isProcessing = false;
        EditorUtility.ClearProgressBar();
        
        occupancyGrid = new ushort[gridSize][][];
    }

    public static void CastInAllDirections(Vector3 origin, int layerMask, float distance, LightProbeGroup group, int depth = 0)
    {
        QueueOperation(origin, new Vector3(0, 1, 0), layerMask, distance, group, depth + 1);
        QueueOperation(origin, new Vector3(0, -1, 0), layerMask, distance, group, depth + 1);
        QueueOperation(origin, new Vector3(1, 0, 0), layerMask, distance, group, depth + 1);
        QueueOperation(origin, new Vector3(-1, 0, 0), layerMask, distance, group, depth + 1);
        QueueOperation(origin, new Vector3(0, 0, -1), layerMask, distance, group, depth + 1);
        QueueOperation(origin, new Vector3(0, 0, 1), layerMask, distance, group, depth + 1);
    }
    
    public static void QueueOperation(Vector3 position, Vector3 direction, int layerMask, float distance, LightProbeGroup group, int depth = 0)
    {
        PendingOperation newOperation = new PendingOperation(position, direction, layerMask, distance, group, depth);
        operationQueue.Add(newOperation);
    }

    public static void ProcessRaycast(Vector3 position, Vector3 direction, int layerMask, float distance, LightProbeGroup group, int depth = 0)
    {
        if (depth > 1000)
        {
            return;
        }
        
        Vector3 targetPosition = position + direction * distance;
        
        if (!Physics.Raycast(position, new Vector3(0, -1, 0), out hitInfo, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        if (!Physics.Raycast(position, direction, out hitInfo, distance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 localPos = targetPosition - group.transform.position;
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);
            int z = Mathf.RoundToInt(localPos.z);
            
            if (Math.Abs(y) > 30)
            {
                return;
            }

            int xIndex = x + gridSize / 2;
            int yIndex = y + gridSize / 2;
            int zIndex = z + gridSize / 2;
            
            if (x > gridSize)
            {
                x = gridSize;
            }
            if (y > gridSize)
            {
                y = gridSize;
            }
            if (z > gridSize)
            {
                z = gridSize;
            }
            
            if (occupancyGrid[xIndex] == null)
            {
                occupancyGrid[xIndex] = new ushort[gridSize][];
            }
            if (occupancyGrid[xIndex][yIndex] == null)
            {
                occupancyGrid[xIndex][yIndex] = new ushort[gridSize];
            }
            if (occupancyGrid[xIndex][yIndex][zIndex] != 0)
            {
                return;
            }
            
            occupancyGrid[xIndex][yIndex][zIndex] = 1;
            
            recentPositions.Add(targetPosition);
            if (recentPositions.Count > maxRecentPositions)
            {
                recentPositions.RemoveAt(0);
            }
            CastInAllDirections(targetPosition, layerMask, probeSpacing, group, depth);
        }
        else
        {
            Vector3 surfaceNormal = hitInfo.normal;
            surfaceNormal = -surfaceNormal;
            surfaceNormal *= surfaceOffset;
            Vector3 surfacePoint = hitInfo.point + surfaceNormal;
            AddProbeToGroup(surfacePoint, group);
            
            ushort currentValue = GetGridValueAtPosition(position, group);
            if (currentValue != 2)
            {
                SetGridValueAtPosition(position, group, 2);
                AddProbeToGroup(position, group);
            }
        }
    }
    
    private static void SetGridValueAtPosition(Vector3 position, LightProbeGroup group, ushort value)
    {
        Vector3 localPos = position - group.transform.position;
        int x = Mathf.RoundToInt(localPos.x);
        int y = Mathf.RoundToInt(localPos.y);
        int z = Mathf.RoundToInt(localPos.z);

        int xIndex = x + gridSize / 2;
        int yIndex = y + gridSize / 2;
        int zIndex = z + gridSize / 2;
        
        xIndex = Mathf.Min(xIndex, gridSize - 1);
        yIndex = Mathf.Min(yIndex, gridSize - 1);
        zIndex = Mathf.Min(zIndex, gridSize - 1);
        
        if (occupancyGrid[xIndex] == null)
        {
            occupancyGrid[xIndex] = new ushort[gridSize][];
        }
        if (occupancyGrid[xIndex][yIndex] == null)
        {
            occupancyGrid[xIndex][yIndex] = new ushort[gridSize];
        }
      
        occupancyGrid[xIndex][yIndex][zIndex] = value;
    }
    
    private static ushort GetGridValueAtPosition(Vector3 position, LightProbeGroup group)
    {
        Vector3 localPos = position - group.transform.position;
        int x = Mathf.RoundToInt(localPos.x);
        int y = Mathf.RoundToInt(localPos.y);
        int z = Mathf.RoundToInt(localPos.z);
                
        int xIndex = x + gridSize / 2;
        int yIndex = y + gridSize / 2;
        int zIndex = z + gridSize / 2;
        
        xIndex = Mathf.Min(xIndex, gridSize - 1);
        yIndex = Mathf.Min(yIndex, gridSize - 1);
        zIndex = Mathf.Min(zIndex, gridSize - 1);
        
        if (occupancyGrid[xIndex] == null)
        {
            occupancyGrid[xIndex] = new ushort[gridSize][];
            return 0;
        }
        if (occupancyGrid[xIndex][yIndex] == null)
        {
            occupancyGrid[xIndex][yIndex] = new ushort[gridSize];
            return 0;
        }
                    
        return occupancyGrid[xIndex][yIndex][zIndex];
    }

    private static void AddProbeToGroup(Vector3 worldPosition, LightProbeGroup group)
    {
        Vector3 localPosition = worldPosition - group.transform.position;
        Vector3[] currentProbes = group.probePositions;
        Array.Resize(ref currentProbes, currentProbes.Length + 1);
        currentProbes[currentProbes.Length - 1] = localPosition;
        group.probePositions = currentProbes;
    }

    public class PendingOperation
    {
        public Vector3 startPoint;
        public Vector3 rayDirection;
        public int mask;
        public float rayLength;
        public LightProbeGroup probeGroup;
        public int recursionLevel;

        public PendingOperation(Vector3 startPoint, Vector3 rayDirection, int mask, float rayLength, LightProbeGroup probeGroup, int recursionLevel)
        {
            this.startPoint = startPoint;
            this.rayDirection = rayDirection;
            this.mask = mask;
            this.rayLength = rayLength;
            this.probeGroup = probeGroup;
            this.recursionLevel = recursionLevel;
        }
    }
}