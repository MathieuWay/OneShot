using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Point : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private TextMeshProUGUI text;
	public int ID { get; set; }
	public float Time { get; set; }
	public SpawnPoint _SpawnPoint { get; set; }

	public void Init(int id, float time, SpawnPoint spawnPoint)
	{
		ID = id;
		Time = time;
		_SpawnPoint = spawnPoint;
		text.text = (id + 1).ToString();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		UI_PointController.Instance.SetCurrentPoint(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		UI_PointController.Instance.SetCurrentPoint(null);

		UI_Timeline.Instance.UpdatePointsOrder();
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = new Vector2(position.x, transform.position.y);

		UI_Timeline.Instance.CheckPointOnTimeline(transform);
	}

	public void UpdateTime()
	{
		Time = UI_Timeline.Instance.GetPointTime(transform);
		_SpawnPoint.SetTime(Time);
	}

	public void ChangeID(int id)
	{
		ID = id;
		text.text = (id + 1).ToString();
		_SpawnPoint.SetID(id);
	}
}