using UnityEngine;
using System.Collections;

public class SnapToGrid : MonoBehaviour {
	public bool enable = true;
	public Vector3 step = new Vector3(1.0f, 1.0f, 1.0f);
	public Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);
}