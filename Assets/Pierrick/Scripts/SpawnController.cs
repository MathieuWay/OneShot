using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôle les points de spawn
/// Instancie les points en fonction de l'input du joueur
/// Place les points au-dessus du sol
/// </summary>
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
	public delegate void SpawnPointDelegate(SpawnPoint spawnPoint);
	public event SpawnPointDelegate SpawnPointEvent;
	public event SpawnDelegate SpawnComplete;

	public List<SpawnPoint> SpawnPoints { get; private set; }


	//TODO: Test
	public void RemoveAllPoints()
	{
		for (int i = 0; i < SpawnPoints.Count; i++)
		{
			Destroy(SpawnPoints[i]._GameObject);
		}

		SpawnPoints.Clear();
	}

	public void RemoveLastPoint()
	{
		if (SpawnPoints.Count <= 0) return;

		Destroy(SpawnPoints[SpawnPoints.Count - 1]._GameObject);
		SpawnPoints.RemoveAt(SpawnPoints.Count - 1);

		UI_Timeline.Instance.RemoveLastPoint();
	}

	public int GetRemainingPoints()
	{
		return spawnCount - SpawnPoints.Count;
	}


	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		SpawnPoints = new List<SpawnPoint>();
	}

	private void Start()
	{
		StartCoroutine(SpawnProcess());
	}

	private IEnumerator SpawnProcess()
	{
		while(SpawnPoints.Count < spawnCount)
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

				SpawnPoint spawnPoint = instance.GetComponent<SpawnPoint>();
				spawnPoint.Init(results[0].point);

				SpawnPoints.Add(spawnPoint);

				SpawnPointEvent?.Invoke(spawnPoint);

				Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

				//Debug.Log(spawnPoint.Time + " " + spawnPoint._Position + " " + spawnPoint._GameObject.name);
			}

			if (Input.GetMouseButtonDown(1))
			{
				RemoveLastPoint();
			}

			yield return null;
		}

		SpawnComplete?.Invoke();
	}
}