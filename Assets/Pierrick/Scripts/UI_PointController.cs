using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class UI_PointController : MonoBehaviour
{
	public static UI_PointController Instance { get; private set; }

	private UI_Point currentPoint;


	public void SetCurrentPoint(UI_Point point)
	{
		currentPoint = point;
	}

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Update()
	{
		if(currentPoint != null)
		{
			currentPoint.SetPosition(Input.mousePosition);
			currentPoint.UpdateTime();
		}
	}
}