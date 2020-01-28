using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Point : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public float Time { get; set; }

	public void Init(float time)
	{
		Time = time;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		UI_PointController.Instance.SetCurrentPoint(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		UI_PointController.Instance.SetCurrentPoint(null);
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = new Vector2(position.x, transform.position.y);

		UI_Timeline.Instance.CheckPointOnTimeline(transform);
	}

	public void UpdateTime()
	{
		Time = UI_Timeline.Instance.GetPointTime(transform);

		Debug.Log(Time);
	}
}