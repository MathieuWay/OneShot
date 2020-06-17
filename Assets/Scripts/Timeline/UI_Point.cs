﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class UI_Point : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private TextMeshProUGUI text = null;
	[SerializeField] private Image pointImage = null;
	[SerializeField] private Sprite pointSelectedSprite = null;
	[SerializeField] private Sprite pointUnselectedSprite = null;
	public int _ID { get; set; }
	public float _Time { get; set; }
	public SpawnPoint _SpawnPoint { get; set; }
    /*
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }*/

    public void Init(int id, float time, SpawnPoint spawnPoint)
	{
		_ID = id;
		_Time = time;
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

	//!NEW FOR GAMEPAD
	public void MovePosition(float dir, float speed)
	{
		transform.position = new Vector2(transform.position.x + dir * speed * Time.deltaTime, transform.position.y);

		UI_Timeline.Instance.CheckPointOnTimeline(transform);
	}
	public void Select()
	{
		pointImage.sprite = pointSelectedSprite;
		//pointImage.color = new Color(1, 0.7f, 0);
		_SpawnPoint.Select();
	}
	public void Unselect()
	{
		pointImage.sprite = pointUnselectedSprite;
		//pointImage.color = Color.red;
		_SpawnPoint.Unselect();
	}
	public void SelectUIPointOnly()
	{
		pointImage.sprite = pointSelectedSprite;
		//pointImage.color = new Color(1, 0.7f, 0);
	}

	public void UpdateTime()
	{
		_Time = UI_Timeline.Instance.GetPointTime(transform);
		_SpawnPoint.SetTime(_Time);
	}

	public void ChangeID(int id)
	{
		_ID = id;
		text.text = (id + 1).ToString();
		_SpawnPoint.SetID(id);
	}
}