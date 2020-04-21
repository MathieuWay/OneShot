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
            for (int i = 0; i < pattern.steps.Count; i++)
            {
                if (pattern.steps[i].type == oneShot.StepType.Move)
                {
                    pattern.steps[i].targetPos = Handles.PositionHandle(pattern.steps[i].targetPos, Quaternion.identity);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, new string[] { "steps"});
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
                    return EditorGUIUtility.singleLineHeight * 3 + 10f;
                case oneShot.StepType.Move:
                    return EditorGUIUtility.singleLineHeight * 5 + 10f;
                case oneShot.StepType.Anim:
                    return EditorGUIUtility.singleLineHeight * 3 + 10f;
                default:
                    return EditorGUIUtility.singleLineHeight * 2 + 10f;
            }
        };
        _patternStepsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty patternStepsProperty = serializedObject.FindProperty("steps").GetArrayElementAtIndex(index);
            Rect propertyRect = rect;
            //STEP TYPE
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            propertyRect.y += 5f;
            SerializedProperty patternStepTypeProperty = patternStepsProperty.FindPropertyRelative("type");
            EditorGUI.PropertyField(propertyRect, patternStepTypeProperty);
            //START TIME
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            SerializedProperty patternStartTimeProperty = patternStepsProperty.FindPropertyRelative("startTime");
            EditorGUI.LabelField(propertyRect, "Début de la Step:", patternStartTimeProperty.floatValue.ToString());

            oneShot.StepType stepType = (oneShot.StepType)patternStepTypeProperty.intValue;
            switch (stepType)
            {
                case oneShot.StepType.Idle:
                    propertyRect.y += EditorGUIUtility.singleLineHeight;
                    SerializedProperty patternTargetPosProperty = patternStepsProperty.FindPropertyRelative("duration");
                    EditorGUI.PropertyField(propertyRect, patternTargetPosProperty);
                    break;
                case oneShot.StepType.Move:
                    propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    SerializedProperty patternDurationProperty = patternStepsProperty.FindPropertyRelative("targetPos");
                    EditorGUI.PropertyField(propertyRect, patternDurationProperty);
                    propertyRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    SerializedProperty patternMoveTypeProperty = patternStepsProperty.FindPropertyRelative("moveType");
                    EditorGUI.PropertyField(propertyRect, patternMoveTypeProperty);
                    break;
                case oneShot.StepType.Anim:
                    propertyRect.y += EditorGUIUtility.singleLineHeight;
                    SerializedProperty patternClipProperty = patternStepsProperty.FindPropertyRelative("clip");
                    EditorGUI.PropertyField(propertyRect, patternClipProperty);
                    break;
                default:
                    break;
            }
        };
    }
}
#endif