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

	public delegate void MyDelegate();
	public event MyDelegate OnSetPause;

	[Header("Timer")]
	[SerializeField] private RectTransform timelineRect = null;
	[SerializeField] private Transform timerIndicator = null;
	[SerializeField] private AnimationCurve pointSelectionCurve = null;
	[SerializeField] private float pointSelectionSpeed = 10;
	private float timerDuration;
	private float timer = 0;
	private float lastTime = 0;
	private bool movingTimeline = false;
	public bool TimeElapsed { get; private set; }

	[Header("Point")]
	[SerializeField] private Transform pointContainer = null;
	[SerializeField] private GameObject pointPrefab = null;
	[SerializeField] private TextMeshProUGUI pointCountText = null;
	private List<UI_Point> points;
	public int SelectedPoint { get; private set; }

	[Header("Pause")]
	[SerializeField] private GameObject pauseContainer = null;
	private bool pause = false;
	//OLD
	//[SerializeField] private Image pauseImage = null;
	//[SerializeField] private Sprite pauseSprite = null;
	//[SerializeField] private Sprite playSprite = null;
	//[SerializeField] private Button pauseButton = null;

	[Header("Other")]
	[SerializeField] private GameObject beginCarnageContainer = null;


	public delegate void TimelineReset();
	public static event TimelineReset OnTimelineReset;
	public static event TimelineReset OnTimeElapsed;


	public void SetTime(float time)
	{
		timer = time;
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

    public float GetTimerDuration()
    {
        return timerDuration;
    }

	public void CheckPointOnTimeline(Transform point)
	{
		if (point.localPosition.x > timelineRect.rect.xMax)
		{
			point.localPosition = new Vector2(timelineRect.rect.xMax, point.localPosition.y);
		}
		else if (point.localPosition.x < timelineRect.rect.xMin)
		{
			point.localPosition = new Vector2(timelineRect.rect.xMin, point.localPosition.y);
		}
	}

	public bool CheckPointTimeCorrect()
	{
		for (int i = 0; i < points.Count; i++)
		{
			if(Mathf.Abs(points[i]._Time - timer) < 0.5f)
			{
				return false;
			}
		}

		return true;
	}

	//Supprime le dernier point sur la Timeline + le point de spawn associé sur la map
	public void RemoveSelectedPoint()
	{
		if (points.Count <= 0) return;

		UI_Point point = points[SelectedPoint];
		Destroy(point.gameObject);

		points.RemoveAt(SelectedPoint);

		SelectedPoint--;

		if (points.Count <= 0)
		{
			SelectedPoint = 0;
		}
		else
		{
			if (SelectedPoint < 0) SelectedPoint = 0;

			UpdatePointsOrder();

			UI_PointController.Instance.SetCurrentPoint(points[SelectedPoint]);
		}

		DisplayPointCount();
	}

	public void BeginCarnage()
	{
		beginCarnageContainer.SetActive(false);
	}

	public bool IsMovingTimeBackward()
	{
		if(GameTime.Instance.TimeSpeed != 0)
		{
			return GameTime.Instance.TimeSpeed < 0;
		}
		else
		{
			return UI_PointController.Instance.MovingBack;
		}
	}


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		points = new List<UI_Point>();
		TimeElapsed = false;

		pauseContainer.SetActive(false);
		//OLD
		//pauseButton.onClick.AddListener(PauseToggle);
	}

	private void Start()
	{
		timerDuration = oneShot.TacticsController.Instance.TimelineDuration;

		SpawnController.Instance.SpawnPointEvent += AddPoint;
		SpawnController.Instance.GrabPointEvent += GrabPoint;
		SpawnController.Instance.UngrabPointEvent += UngrabPoint;
		SpawnController.Instance.SpawnFailEvent += delegate { CursorController.Instance.SpawnPointFail(); };

		DisplayPointCount();

		StartCoroutine(TimerProcess());
	}

	private void Update()
	{
		if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat) return;

		//Sélection des points sur la timeline
		if (!movingTimeline && Gamepad.Instance.ButtonDownTriggerR && SelectedPoint < points.Count - 1)
		{
			SelectedPoint++;

			UI_PointController.Instance.SetCurrentPoint(points[SelectedPoint]);
			SpawnController.Instance.CancelGrab();

			MoveTimeline(points[SelectedPoint]._Time);
		}
		if (!movingTimeline && Gamepad.Instance.ButtonDownTriggerL && SelectedPoint > 0)
		{
			SelectedPoint--;

			UI_PointController.Instance.SetCurrentPoint(points[SelectedPoint]);
			SpawnController.Instance.CancelGrab();

			MoveTimeline(points[SelectedPoint]._Time);
		}

		if (Gamepad.Instance.ButtonDownX)
		{
			PauseToggle();

			if (pause) OnSetPause?.Invoke();
		}
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

		SelectedPoint = UI_PointController.Instance.GetCurrentPointID();
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

		UI_PointController.Instance.SetCurrentPoint(uiPoint);
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

	private void MoveTimeline(float time)
	{
		if (movingTimeline) return;

		movingTimeline = true;

		StartCoroutine(MoveTimelineProcess(time));
	}
	private IEnumerator MoveTimelineProcess(float time)
	{
		float duration = Mathf.Abs(time - timer);
		float moveTimer = 0;

		if (timer < time)
		{
			while (timer < time)
			{
				GameTime.Instance.SetHardTimeSpeed(1 + pointSelectionCurve.Evaluate(moveTimer / duration) * pointSelectionSpeed);

				moveTimer += Time.deltaTime * GameTime.Instance.TimeSpeed;
				yield return null;
			}
		}
		else if (timer > time)
		{
			while (timer > time)
			{
				GameTime.Instance.SetHardTimeSpeed(-1 * (1 + pointSelectionCurve.Evaluate(moveTimer / duration) * pointSelectionSpeed));

				moveTimer += Time.deltaTime * Mathf.Abs(GameTime.Instance.TimeSpeed);
				yield return null;
			}
		}

		timer = time;
		SetOnTimeline(timerIndicator, timer / timerDuration);
		SetPause(true);

		movingTimeline = false;
	}

	private IEnumerator TimerProcess()
	{
		while (true)
		{
			timer += Time.deltaTime * GameTime.Instance.TimeSpeed;

			if (timer >= timerDuration)
			{
				if(oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
				{
					TimeElapsed = true;
					OnTimeElapsed?.Invoke();
					yield break;
				}
				else
				{
					ResetTimeline();
				}
			}

			SetOnTimeline(timerIndicator, timer / timerDuration);

			yield return null;
		}
	}

	public void PauseToggle()
	{
		SetPause(!pause);
	}

	public void SetPause(bool state)
	{
		pause = state;
		pauseContainer.SetActive(pause);

		GameTime.Instance.SetHardTimeSpeed(pause ? 0 : 1);

		//OLD
		//pauseImage.sprite = pause ? playSprite : pauseSprite;
	}

	public void ResetTimeline()
	{
		timer = 0;
		SetOnTimeline(timerIndicator, timer / timerDuration);
		SetPause(false);
		OnTimelineReset?.Invoke();
	}

	public void UpdatePointsOrder()
	{
		points = points.OrderBy(o => o._Time).ToList();

		for (int i = 0; i < points.Count; i++)
		{
			points[i].ChangeID(i);
		}

		SpawnController.Instance.ChangeSpawnPointsOrder();
	}

	public void UpdateCurrentPointSelected()
	{
		SelectedPoint = UI_PointController.Instance.GetCurrentPointID();
	}

	private void GrabPoint(SpawnPoint point)
	{
		if(point._ID != UI_PointController.Instance.GetCurrentPointID())
		{
			UI_PointController.Instance.UnselectCurrentPoint();
		}

		SelectedPoint = point._ID;
		UI_PointController.Instance.SelectPoint(points[SelectedPoint]);

		SetTime(points[SelectedPoint]._Time);
		SetPause(true);
	}
	private void UngrabPoint(SpawnPoint point)
	{
		SelectedPoint = point._ID;
		UI_PointController.Instance.SetCurrentPoint(points[SelectedPoint]);
	}

	//OLD
	//public void ResetAll()
	//{
	//	while (points.Count > 0)
	//	{
	//		RemoveSelectedPoint();
	//	}

	//	timer = 0;

	//	SetOnTimeline(timerIndicator, timer / timerDuration);

	//	pause = true;
	//}

	//OLD VERSION: Prend en compte la taille de l'image pour ne pas dépasser les bordures de la timeline
	//
	//private void SetOnTimeline(Transform point, float value)
	//{
	//	point.localPosition = new Vector2((timelineRect.rect.xMin + (point.GetComponent<RectTransform>().sizeDelta.x / 2)) +
	//		value * ((timelineRect.rect.xMax - timelineRect.rect.xMin) - point.GetComponent<RectTransform>().sizeDelta.x),
	//		timelineRect.localPosition.y);
	//}
}
