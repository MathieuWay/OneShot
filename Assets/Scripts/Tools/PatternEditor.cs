using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(oneShot.Pattern), true)]
public class PatternEditor : Editor
{
    private void OnSceneGUI()
    {
        oneShot.Pattern pattern = (oneShot.Pattern)target;
        if (pattern.editPattern)
        {
            for (int i = 0; i < pattern.steps.Count; i++)
            {
                if (pattern.steps[i].type == oneShot.StepType.Move)
                {
                    pattern.steps[i].targetPos = Handles.PositionHandle(pattern.steps[i].targetPos, Quaternion.identity);
                }
            }
        }
    }
}
#endif