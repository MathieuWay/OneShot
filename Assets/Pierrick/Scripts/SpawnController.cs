using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	public static SpawnController instance;
	[SerializeField] private Camera cam;
	[SerializeField] private GameObject pointPrefab;
	[SerializeField] private int spawnCount = 10;
	[SerializeField] private string[] blockerTags;

	public delegate void SpawnDelegate();
	public event SpawnDelegate SpawnPointEvent;
	public event SpawnDelegate SpawnComplete;

	private Queue<GameObject> points;


	//TODO: Test
	public void RemoveAllPoints()
	{
		while (points.Count > 0)
		{
			GameObject point = points.Dequeue();

			Destroy(point);
		}
	}

	public int GetRemainingPoints()
	{
		return spawnCount - points.Count;
	}


	private void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		points = new Queue<GameObject>();
	}

	private void Start()
	{
		StartCoroutine(SpawnProcess());
	}

	private IEnumerator SpawnProcess()
	{
		while(points.Count < spawnCount)
		{
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

				if(hit.collider != null)
				{
					bool hitBlocker = false;

					for (int i = 0; i < blockerTags.Length; i++)
					{
						if(hit.transform.CompareTag(blockerTags[i]))
						{
							hitBlocker = true;
						}
					}

					Debug.Log(hit.transform.tag + " " + hit.transform.name);

					if (hitBlocker)
					{
						yield return null;
						continue;
					}
						

					GameObject instance = Instantiate(pointPrefab, hit.point, Quaternion.identity);

					points.Enqueue(instance);

					SpawnPointEvent?.Invoke();

					Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
				}
			}

			yield return null;
		}

		SpawnComplete?.Invoke();
	}
}
