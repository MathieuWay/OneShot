using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(InterLayer), true)]
public class InterLayerEditor : Editor
{
    private void OnSceneGUI()
    {
        InterLayer interlayer = (InterLayer)target;
        if (interlayer.editPath)
        {
            for (int i = 0; i < interlayer.paths.Count; i++)
            {
                interlayer.paths[i].path.waypoint = interlayer.transform.InverseTransformPoint(Handles.PositionHandle(interlayer.transform.TransformPoint(interlayer.paths[i].path.waypoint), Quaternion.identity));
            }
        }
    }
}
#endif
