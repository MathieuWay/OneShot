using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(InterLayer), true)]
public class InterLayerEditor : Editor
{
    ReorderableList _pathReorderableList;

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
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, new string[] { "paths" });
        _pathReorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        SerializedProperty patternsStepsProperty = serializedObject.FindProperty("paths");
        _pathReorderableList = new ReorderableList(serializedObject, patternsStepsProperty);
        _pathReorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 10f;
        _pathReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty patternStepsProperty = serializedObject.FindProperty("paths").GetArrayElementAtIndex(index);
            Rect propertyRect = rect;
            //STEP TYPE
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty patternPathProperty = patternStepsProperty.FindPropertyRelative("path");
            //patternPathProperty.serializedObject.FindProperty("waypoint");
            //EditorGUI.PropertyField(propertyRect, patternPathProperty.FindPropertyRelative("waypoint"));
            EditorGUI.Vector3Field(propertyRect, "Position:", patternPathProperty.FindPropertyRelative("waypoint").vector3Value);
            //START TIME
            propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
            SerializedProperty patternSpeedProperty = patternStepsProperty.FindPropertyRelative("speed");
            EditorGUI.FloatField(propertyRect, "Speed:", patternSpeedProperty.floatValue);
        };
    }
}
#endif
