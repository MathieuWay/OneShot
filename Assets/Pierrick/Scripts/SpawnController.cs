using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	public static SpawnController Instance { get; private set; }

	[SerializeField] private Camera cam;
	[SerializeField] private GameObject pointPrefab;
	[SerializeField] private int spawnCount = 10;
	[SerializeField] private string[] blockerTags;
	[SerializeField] private float detectionDistanceToFloor = 3;
	[SerializeField] private float spawnDistanceToFloor = 0.3f;

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
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

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

				if(hit.collider == null)
				{
					yield return null;
					continue;
				}

				bool hitBlocker = false;

				//Check tous les tags considérés comme "bloqueur"
				for (int i = 0; i < blockerTags.Length; i++)
				{
					if (hit.transform.CompareTag(blockerTags[i]))
					{
						hitBlocker = true;
					}
				}

				if (hitBlocker)
				{
					yield return null;
					continue;
				}

				//Détection du sol sans prendre en compte les Triggers
				ContactFilter2D contactFilter = new ContactFilter2D() { useTriggers = false };
				RaycastHit2D[] results = new RaycastHit2D[1];
				int hitGround = Physics2D.Raycast(hit.point, Vector2.down, contactFilter, results, detectionDistanceToFloor);

				if (hitGround <= 0)
				{
					//Si aucune détection de collider, aucun sol n'a été trouvé
					yield return null;
					continue;
				}

				//Instance du point de spawn sur la surface touché
				GameObject instance = Instantiate(pointPrefab, results[0].point + Vector2.up * spawnDistanceToFloor, Quaternion.identity);

				points.Enqueue(instance);

				SpawnPointEvent?.Invoke();

				Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
			}

			yield return null;
		}

		SpawnComplete?.Invoke();
	}
}