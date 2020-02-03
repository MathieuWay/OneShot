using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public float Time { get; private set; }
	public Vector2 _Position { get; private set; }
	public GameObject _GameObject { get; private set; }

	public void Init(Vector3 root)
	{
		_Position = root;
		_GameObject = gameObject;
		Time = UI_Timeline.Instance.GetCurrentTime();
	}

	public void SetTime(float time)
	{
		Time = time;
	}
}
