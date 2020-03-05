using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof (SnapToGrid))]
[CanEditMultipleObjects]
public class SnapToGridEditor : Editor {
	
	SnapToGrid snapScript;
	
	void OnEnable () {
		snapScript = target as SnapToGrid;
	}
	
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(GUI.changed) {
			RefreshPosition();
		}
	}
	
	public void OnSceneGUI() {
		RefreshPosition();
	}
	
	protected void RefreshPosition() {
		if(!snapScript.enable)
			return;

		Vector3 position = snapScript.transform.position;
		if(snapScript.step.x > 0.0f) {
			float stepToX = (position.x - snapScript.offset.x) / snapScript.step.x;
			position.x = snapScript.step.x * Mathf.Round (stepToX) + snapScript.offset.x;
		}
		if(snapScript.step.y > 0.0f) {
			float stepToY = (position.y - snapScript.offset.y) / snapScript.step.y;
			position.y = snapScript.step.y * Mathf.Round (stepToY) + snapScript.offset.y;
		}
        if (snapScript.step.z > 0.0f)
        {
            float stepToZ = (position.z - snapScript.offset.z) / snapScript.step.z;
            position.z = snapScript.step.z * Mathf.Round(stepToZ) + snapScript.offset.z;
        }

        snapScript.transform.position = position;
	}
}
