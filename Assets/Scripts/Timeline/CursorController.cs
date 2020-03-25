using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CursorController : MonoBehaviour
{
	public static CursorController Instance { get; private set; }

	//[Header("Cursor Selection")]
	//[SerializeField] private GraphicRaycaster m_Raycaster;
	//[SerializeField] private EventSystem m_EventSystem;
	//private PointerEventData m_PointerEventData;
	
	[Header("Cursor Settings")]
	[SerializeField] private Transform canvasTransform = null;
	[SerializeField] private Transform cursorPoint = null;
	[SerializeField] private Image cursorImage = null;
	[SerializeField] private Color cursorFailColor = Color.white;
	[SerializeField] private float speed = 100;
	[SerializeField] private float accelerationSpeed = 5;
	[SerializeField] private float maxAcceleration = 10;
	private Color initCursorColor;
	private bool isFailing;
	private float acceleration;
	private float radius;


	public Vector2 GetPosition()
	{
		return cursorPoint.position;
	}

	public void SetState(bool state)
	{
		cursorPoint.gameObject.SetActive(state);
	}

	public void SpawnPointFail()
	{
		if (isFailing) return;

		cursorImage.color = cursorFailColor;
		StartCoroutine(SpawnPointFailDuration());
	}
	private IEnumerator SpawnPointFailDuration()
	{
		isFailing = true;
		yield return new WaitForSeconds(0.5f);
		cursorImage.color = initCursorColor;
		isFailing = false;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		initCursorColor = cursorImage.color;
	}

	private void Start()
	{
		//Récupération du rayon du curseur en prenant en compte la taille du canvas
		radius = (cursorPoint.GetComponent<RectTransform>().sizeDelta.x / 2) * canvasTransform.localScale.x;
	}

	private void Update()
	{
		if (Gamepad.Instance.HorizontalJL != 0 || Gamepad.Instance.VerticalJL != 0)
		{
			if (acceleration > maxAcceleration)
			{
				acceleration = maxAcceleration;
			}
			else
			{
				acceleration += Time.deltaTime * accelerationSpeed;
			}

			cursorPoint.position += new Vector3(Gamepad.Instance.HorizontalJL, Gamepad.Instance.VerticalJL, 0) * speed * acceleration * Time.deltaTime;

			if (cursorPoint.position.x > Screen.width - radius)
			{
				cursorPoint.position = new Vector2(Screen.width - radius, cursorPoint.position.y);
			}
			if (cursorPoint.position.y > Screen.height - radius)
			{
				cursorPoint.position = new Vector2(cursorPoint.position.x, Screen.height - radius);
			}
			if (cursorPoint.position.x < radius)
			{
				cursorPoint.position = new Vector2(radius, cursorPoint.position.y);
			}
			if (cursorPoint.position.y < radius)
			{
				cursorPoint.position = new Vector2(cursorPoint.position.x, radius);
			}
		}
		else
		{
			acceleration = 1;
		}

		//Selection();
	}

	//private void Selection()
	//{
	//	if (Gamepad.Instance.ButtonDownA)
	//	{
	//		m_PointerEventData = new PointerEventData(m_EventSystem);
	//		m_PointerEventData.position = cursorPoint.position;

	//		List<RaycastResult> results = new List<RaycastResult>();

	//		m_Raycaster.Raycast(m_PointerEventData, results);

	//		foreach (RaycastResult result in results)
	//		{
	//			Debug.Log("Hit " + result.gameObject.name);
	//		}
	//	}
	//}
}