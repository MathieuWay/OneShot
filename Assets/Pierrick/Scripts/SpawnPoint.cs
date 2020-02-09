using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnPoint : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI numberText;
	public int _ID { get; set; }
	public float _Time { get; private set; }
	public Vector2 _Position { get; private set; }
	public GameObject _GameObject { get; private set; }

	public void Init(int id, Vector3 root)
	{
		SetID(id);
		_Position = root;
		_GameObject = gameObject;
		_Time = UI_Timeline.Instance.GetCurrentTime();
	}

	public void SetTime(float time)
	{
		_Time = time;
	}

	public void SetID(int id)
	{
		_ID = id;
		numberText.text = (id + 1).ToString();
	}
}
