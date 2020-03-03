using UnityEngine;
using UnityEngine.UI;


public class CursorController : MonoBehaviour
{
	public static CursorController Instance { get; private set; }

	[SerializeField] private Transform canvasTransform;
	[SerializeField] private Transform cursorPoint;
	[SerializeField] private float speed = 100;
	[SerializeField] private float accelerationSpeed = 5;
	[SerializeField] private float maxAcceleration = 10;
	private float acceleration;
	private float radius;


	public Vector2 GetPosition()
	{
		return cursorPoint.position;
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
	}
}