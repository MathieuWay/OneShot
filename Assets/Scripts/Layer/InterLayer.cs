﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct AccessPath
{
    public oneShot.StepMovePath path;
    public float speed;
}

[ExecuteInEditMode]
public class InterLayer : MonoBehaviour
{
    [ReorderableList]
    public List<AccessPath> paths = new List<AccessPath>();
    public bool showPath = false;
    public bool editPath = false;

    public List<oneShot.StepMovePath> LoadPath()
    {
        List<oneShot.StepMovePath> accessPath = new List<oneShot.StepMovePath>();
        foreach (AccessPath path in paths)
        {
            accessPath.Add(new oneShot.StepMovePath(transform.TransformPoint(path.path.waypoint), path.path.time));
        }
        return accessPath;
    }


    public void OnValidate()
    {
        if(paths.Count > 0)
        {
            float duration = 0;
            Vector3 pos = paths[0].path.waypoint;
            paths[0].path.time = 0;
            for (int i = 1; i < paths.Count; i++)
            {
                duration += Vector3.Distance(pos, paths[i].path.waypoint) / paths[i-1].speed;
                paths[i].path.time = duration;
                pos = paths[i].path.waypoint;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showPath)
        {
            Gizmos.color = Color.green;//transform.TransformVector(paths[i - 1].path.waypoint)
            for (int i = 0; i < paths.Count; i++)
            {
                Vector3 localPos = transform.TransformPoint(paths[i].path.waypoint);
                //if (editPath)
                //    paths[i].path.waypoint = transform.InverseTransformPoint(Handles.PositionHandle(transform.TransformPoint(paths[i].path.waypoint), Quaternion.identity));
                Gizmos.DrawSphere(localPos, 0.1f);
                if(i < paths.Count - 1)
                    Gizmos.DrawLine(localPos, transform.TransformPoint(paths[i+1].path.waypoint));
            }
        }
    }
#endif
}
