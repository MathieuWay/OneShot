using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
        Gizmos.color = Color.green;//transform.TransformVector(paths[i - 1].path.waypoint)
        for (int i = 1; i < paths.Count; i++) Gizmos.DrawLine(transform.TransformPoint(paths[i - 1].path.waypoint), transform.TransformPoint(paths[i].path.waypoint));
    }
#endif
}
