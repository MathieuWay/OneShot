﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Timeline : MonoBehaviour
{
	public static UI_Timeline Instance { get; private set; }

	[Header("Timer")]
	[SerializeField] private RectTransform timelineRect;
	[SerializeField] private Transform timerIndicator;
	[SerializeField] private float timerDuration = 10;
	private float timer = 0;

	[Header("Point")]
	[SerializeField] private Transform pointContainer;
	[SerializeField] private GameObject pointPrefab;
	[SerializeField] private TextMeshProUGUI pointCountText;
	private Queue<GameObject> points;

	[Header("Pause")]
	[SerializeField] private Button pauseButton;
	[SerializeField] private Image pauseImage;
	[SerializeField] private Sprite pauseSprite;
	[SerializeField] private Sprite playSprite;
	private bool pause = false;


	//TODO: To Test
	public void ResetAll()
	{
		while (points.Count > 0)
		{
			RemovePoint();
		}

		timer = 0;

		SetOnTimeline(timerIndicator, timer / timerDuration);

		pause = true;
	}

	public float GetPointTime(Transform point)
	{
		//point.localPosition = new Vector2(timelineRect.rect.xMin + value * (timelineRect.rect.xMax - timelineRect.rect.xMin), 0);

		float value = (point.localPosition.x - timelineRect.rect.xMin) / (timelineRect.rect.xMax - timelineRect.rect.xMin);

		return value * timerDuration;
	}

	public void CheckPointOnTimeline(Transform point)
	{
		if(point.localPosition.x > timelineRect.rect.xMax)
		{
			point.localPosition = new Vector2(timelineRect.rect.xMax, point.localPosition.y);
		}
		else if(point.localPosition.x < timelineRect.rect.xMin)
		{
			point.localPosition = new Vector2(timelineRect.rect.xMin, point.localPosition.y);
		}
	}


	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		points = new Queue<GameObject>();

		pauseButton.onClick.AddListener(PauseToggle);
	}

	private void Start()
	{
		SpawnController.Instance.SpawnPointEvent += AddPoint;

		pointCountText.text = SpawnController.Instance.GetRemainingPoints().ToString();

		StartCoroutine(TimerProcess());
	}

	private void AddPoint()
	{
		SetPoint(timer / timerDuration);

		pointCountText.text = SpawnController.Instance.GetRemainingPoints().ToString();
	}

	private void RemovePoint()
	{
		GameObject point = points.Dequeue();

		Destroy(point);
	}

	public void SetPoint(float value)
	{
		GameObject instance = Instantiate(pointPrefab, pointContainer);

		instance.GetComponent<UI_Point>().Init(timer);

		points.Enqueue(instance);

		SetOnTimeline(instance.transform, value);
	}

	/// <summary>
	/// Permet de placer un Transform sur la Timeline.
	/// </summary>
	/// <param name="point">Le Transform</param>
	/// <param name="value">Le pourcentage de la Timeline, entre 0 et 1</param>
	private void SetOnTimeline(Transform point, float value)
	{
		point.localPosition = new Vector2(timelineRect.rect.xMin + value * (timelineRect.rect.xMax - timelineRect.rect.xMin), 0);
	}

	private IEnumerator TimerProcess()
	{
		while(true)
		{
			if (pause)
			{
				yield return null;
				continue;
			}

			timer += Time.deltaTime;

			if(timer >= timerDuration) timer = 0;

			SetOnTimeline(timerIndicator, timer / timerDuration);

			yield return null;
		}
	}

	private void PauseToggle()
	{
		pause = !pause;

		pauseImage.sprite = pause ? playSprite : pauseSprite;
	}



	//OLD VERSION: Prend en compte la taille de l'image pour ne pas dépasser les bordures de la timeline
	//
	//private void SetOnTimeline(Transform point, float value)
	//{
	//	point.localPosition = new Vector2((timelineRect.rect.xMin + (point.GetComponent<RectTransform>().sizeDelta.x / 2)) +
	//		value * ((timelineRect.rect.xMax - timelineRect.rect.xMin) - point.GetComponent<RectTransform>().sizeDelta.x),
	//		timelineRect.localPosition.y);
	//}
}