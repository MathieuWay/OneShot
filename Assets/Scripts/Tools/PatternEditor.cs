using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(oneShot.Pattern), true)]
public class PatternEditor : Editor
{
    ReorderableList _patternStepsReorderableList;

    private void OnSceneGUI()
    {
        oneShot.Pattern pattern = (oneShot.Pattern)target;
        if (pattern.editPattern)
        {
            serializedObject.Update();
            for (int i = 0; i < pattern.steps.Count; i++)
            {
                if (pattern.steps[i].type == oneShot.StepType.Move)
                {
                    serializedObject.FindProperty("steps").GetArrayElementAtIndex(i).FindPropertyRelative("targetPos").vector3Value = Handles.PositionHandle(pattern.steps[i].targetPos, Quaternion.identity);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        string[] filter;
        if (serializedObject.FindProperty("showPattern").boolValue)
            filter = new string[] { "steps" };
        else
            filter = new string[] { "steps", "debugColor" };
        DrawPropertiesExcluding(serializedObject, filter);
        _patternStepsReorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        SerializedProperty patternsStepsProperty = serializedObject.FindProperty("steps");
        _patternStepsReorderableList = new ReorderableList(serializedObject, patternsStepsProperty);
        //_patternStepsReorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 3 + 10f;
        _patternStepsReorderableList.elementHeightCallback = (int index) =>
        {
            SerializedProperty patternStepsProperty = serializedObject.FindProperty("steps").GetArrayElementAtIndex(index);
            SerializedProperty patternStepTypeProperty = patternStepsProperty.FindPropertyRelative("type");
            oneShot.StepType stepType = (oneShot.StepType)patternStepTypeProperty.intValue;
            switch (stepType)
            {
                case oneShot.StepType.Idle:
                    return EditorGUIUtility.singleLineHeight * 3 + 20f;
                case oneShot.StepType.Move:
                    return EditorGUIUtility.singleLineHeight * 5 + 10f;
                case oneShot.StepType.Anim:
                    return EditorGUIUtility.singleLineHeight * 3 + 20f;
                default:
                    return EditorGUIUtility.singleLineHeight * 2 + 15f;
            }
        };
        _patternStepsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty patternStepsProperty = serializedObject.FindProperty("steps").GetArrayElementAtIndex(index);
            Rect propertyRect = rect;
            // Step Type
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            propertyRect.y += 5f;
            SerializedProperty patternStepTypeProperty = patternStepsProperty.FindPropertyRelative("type");
            EditorGUI.PropertyField(propertyRect, patternStepTypeProperty, new GUIContent("Step Type"));
            // Start time step
            propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
            EditorGUI.FloatField(propertyRect, "Start at", patternStepsProperty.FindPropertyRelative("startTime").floatValue);

            oneShot.StepType stepType = (oneShot.StepType)patternStepTypeProperty.intValue;
            switch (stepType)
            {
                case oneShot.StepType.Idle:
                    // Idle Duration
                    propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    EditorGUI.PropertyField(propertyRect, patternStepsProperty.FindPropertyRelative("duration"), new GUIContent("Duration"));
                    break;
                case oneShot.StepType.Move:
                    // Position target
                    propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    EditorGUI.PropertyField(propertyRect, patternStepsProperty.FindPropertyRelative("targetPos"), new GUIContent("Position"));
                    // Move Type
                    propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    EditorGUI.PropertyField(propertyRect, patternStepsProperty.FindPropertyRelative("moveType"), new GUIContent("Move Type"));
                    break;
                case oneShot.StepType.Anim:
                    // Animation clip
                    propertyRect.y += EditorGUIUtility.singleLineHeight +5f;
                    EditorGUI.PropertyField(propertyRect, patternStepsProperty.FindPropertyRelative("clip"), new GUIContent("Animation Clip"));
                    break;
                default:
                    break;
            }
        };
    }
}
#endif