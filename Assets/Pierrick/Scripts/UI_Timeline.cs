using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Timeline : MonoBehaviour
{
	[SerializeField] private RectTransform timelineRect;
	[SerializeField] private Transform pointContainer;
	[SerializeField] private GameObject pointPrefab;
	[SerializeField] private TextMeshProUGUI pointCountText;
	private float x = 0;

	private void Start()
	{
		SpawnController.instance.SpawnPointEvent += AddPoint;

		pointCountText.text = SpawnController.instance.GetRemainingPoints().ToString();
	}

	private void AddPoint()
	{
		x += 0.1f;
		SetPoint(x);

		pointCountText.text = SpawnController.instance.GetRemainingPoints().ToString();
	}

	public void SetPoint(float value)
	{
		GameObject instance = Instantiate(pointPrefab, pointContainer);

		instance.transform.localPosition = new Vector2((timelineRect.rect.xMin + (instance.GetComponent<RectTransform>().sizeDelta.x/2)) + 
			value * ((timelineRect.rect.xMax - timelineRect.rect.xMin) - instance.GetComponent<RectTransform>().sizeDelta.x),
			timelineRect.localPosition.y);
	}
}
