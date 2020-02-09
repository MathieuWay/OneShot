using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


/// <summary>
/// Gère le placement des points sur la timeline
/// Instancie les points avec la classe UI_Point: time + position
/// Contrôle le timer et sa position sur la timeline
/// </summary>
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
	private List<UI_Point> points;

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
			RemoveLastPoint();
		}

		timer = 0;

		SetOnTimeline(timerIndicator, timer / timerDuration);

		pause = true;
	}

	public float GetPointTime(Transform point)
	{
		float value = (point.localPosition.x - timelineRect.rect.xMin) / (timelineRect.rect.xMax - timelineRect.rect.xMin);

		return value * timerDuration;
	}

	public float GetCurrentTime()
	{
		return timer;
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

	//Supprime le dernier point sur la Timeline + le point de spawn associé sur la map
	public void RemoveLastPoint()
	{
		if (points.Count <= 0) return;

		UI_Point point = points[points.Count - 1];
		Destroy(point.gameObject);

		points.RemoveAt(points.Count - 1);

		DisplayPointCount();
	}


	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		points = new List<UI_Point>();

		pauseButton.onClick.AddListener(PauseToggle);
	}

	private void Start()
	{
		SpawnController.Instance.SpawnPointEvent += AddPoint;

		DisplayPointCount();

		StartCoroutine(TimerProcess());
	}

	private void DisplayPointCount()
	{
		pointCountText.text = SpawnController.Instance.GetRemainingPoints().ToString();
	}

	//Ajoute un point sur la timeline avec son point de spawn
	private void AddPoint(SpawnPoint spawnPoint)
	{
		SetPoint(timer / timerDuration, spawnPoint);

		pointCountText.text = SpawnController.Instance.GetRemainingPoints().ToString();

		UpdatePointsOrder();
	}

	//Met un point sur la timeline + associe son point de spawn
	private void SetPoint(float value, SpawnPoint spawnPoint)
	{
		GameObject instance = Instantiate(pointPrefab, pointContainer);

		UI_Point uiPoint = instance.GetComponent<UI_Point>();

		//L'ID correspond au nombre de point - 1 (pour commencer à partir de 0)
		uiPoint.Init((SpawnController.Instance.SpawnPoints.Count - 1), timer, spawnPoint);

		points.Add(uiPoint);

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

	public void PauseToggle()
	{
		pause = !pause;

		pauseImage.sprite = pause ? playSprite : pauseSprite;
	}

    public void SetPause(bool state)
    {
        pause = state;
        pauseImage.sprite = pause ? playSprite : pauseSprite;
    }

    public void ResetTimeline()
    {
        timer = 0;
        SetOnTimeline(timerIndicator, timer / timerDuration);
        pause = false;
    }

	public void UpdatePointsOrder()
	{
		points = points.OrderBy(o => o.Time).ToList();

		for (int i = 0; i < points.Count; i++)
		{
			points[i].ChangeID(i);
		}

		SpawnController.Instance.ChangeSpawnPointsOrder();
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