using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class UI_PointController : MonoBehaviour
{
	public static UI_PointController Instance { get; private set; }

	[SerializeField] private float movePointSpeed = 200f;
	private UI_Point currentPoint;
	private bool pointMoved;


	public void SetCurrentPoint(UI_Point point)
	{
		if (currentPoint != null) currentPoint.Unselect();

		currentPoint = point;
		currentPoint.Select();
	}

	public void UnselectCurrentPoint()
	{
		currentPoint.Unselect();
	}
	public void SelectPoint(UI_Point point)
	{
		currentPoint = point;
		currentPoint.SelectUIPointOnly();
	}

	public int GetCurrentPointID()
	{
		return currentPoint._ID;
	}

	public bool DraggingPoint()
	{
		return currentPoint != null;
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

	private void Update()
	{
		if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat) return;

		if (currentPoint != null)
		{
			//!FOR MOUSE & KEYBOARD
			//currentPoint.SetPosition(Input.mousePosition);

			//!NEW FOR GAMEPAD
			if (Gamepad.Instance.HorizontalJR != 0)
			{
				currentPoint.MovePosition(Gamepad.Instance.HorizontalJR, movePointSpeed);
				pointMoved = true;
				UI_Timeline.Instance.SetTime(currentPoint._Time);
				UI_Timeline.Instance.SetPause(true);
			}
			else if (pointMoved)
			{
				UI_Timeline.Instance.UpdatePointsOrder();
				UI_Timeline.Instance.UpdateCurrentPointSelected();
				pointMoved = false;
			}

			currentPoint.UpdateTime();
		}
	}
}