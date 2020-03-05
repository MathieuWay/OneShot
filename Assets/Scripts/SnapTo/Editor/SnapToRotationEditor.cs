using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SnapToRotation))]
public class SnapToRotationEditor : Editor {

	SnapToRotation snapScript;
	
	void OnEnable() {
		snapScript = target as SnapToRotation;
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
		
		Vector3 eulerAngles = snapScript.transform.eulerAngles;
		if(snapScript.step > 0.0f) {
			float stepToZ = (eulerAngles.z - snapScript.offset) / snapScript.step;
			eulerAngles.x = 0.0f;
			eulerAngles.y = 0.0f;
			eulerAngles.z = snapScript.step * Mathf.Round(stepToZ) + snapScript.offset;
		}
		
		snapScript.transform.eulerAngles = eulerAngles;
	}
}

