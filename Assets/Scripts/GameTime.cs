using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class GameTime : MonoBehaviour
{
	public static GameTime Instance { get; private set; }
	public float TimeSpeed { get; private set; }
	public delegate void TimeDelegate();
	public event TimeDelegate OnStartSlowMotion;


	public void SetHardTimeSpeed(float speed)
	{
		TimeSpeed = speed;
	}

	//#Critical: Ne le faire uniquement pour des cas très spécifiques (exemple -> tutoriel)
	public void CancelCoroutine()
	{
		StopAllCoroutines();
	}

	public void SetTimeSpeed(float speed, float duration)
	{
		StartCoroutine(SetTimeSpeedProcess(speed, duration));
	}

	public void SlowMotion(float speed, float duration)
	{
		if (speed > 1)
		{
			speed = 1;
		}
		else if (speed < 0)
		{
			speed = 0;
		}

		SetTimeSpeed(speed, duration);

		OnStartSlowMotion?.Invoke();
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