using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class GameTime : MonoBehaviour
{
	public static GameTime Instance { get; private set; }
	public float TimeSpeed { get; private set; }

	public void SetTimeSpeed(float speed, float duration)
	{
		if (speed > 1)
		{
			speed = 1;
		}
		else if (speed < 0)
		{
			speed = 0;
		}

		StartCoroutine(SetTimeSpeedProcess(speed, duration));
	}


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Start()
	{
		TimeSpeed = 1;
	}

	private IEnumerator SetTimeSpeedProcess(float speed, float duration)
	{
		TimeSpeed = speed;

		yield return new WaitForSeconds(duration);

		TimeSpeed = 1;
	}
}