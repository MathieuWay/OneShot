using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

	public void ChangeSpawnPointsOrder()
	{
		SpawnPoints = SpawnPoints.OrderBy(o => o._Time).ToList();
	}


	private void Awake()
	{
		if (Instance != null)
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
		while (oneShot.LevelController.Instance.phase == oneShot.Phase.Tactical && SpawnPoints.Count < spawnCount)
		{
			if (/*Input.GetMouseButtonDown(0)*/Gamepad.Instance.ButtonDownA /*&& !UI_PointController.Instance.DraggingPoint()*/)
			{
				//!OLD : Fonctionne uniquement avec une camera en vue orthographique
				//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(CursorController.Instance.GetPosition()/*Input.mousePosition*/), Vector2.zero);


				Ray ray = Camera.main.ScreenPointToRay(CursorController.Instance.GetPosition());

				RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

				if (hit.collider == null)
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


				//!OLD: DETECTION 2D
				//Détection du sol sans prendre en compte les Triggers
				/*
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

				//L'ID correspond au nombre de point de spawn, on commence ici par 0
				spawnPoint.Init(SpawnPoints.Count, results[0].point);
				*/
				//!FIN


				//!DETECTION 3D
				RaycastHit hitFloor;
				Physics.Raycast(hit.point, Vector3.down, out hitFloor, detectionDistanceToFloor);

				if (hitFloor.transform == null)
				{
					//Si aucune détection de collider, aucun sol n'a été trouvé
					yield return null;
					continue;
				}

				Vector3 spawnPosition = hitFloor.point + Vector3.up * spawnDistanceToFloor;

				//Instance du point de spawn sur la surface touché
				GameObject instance = Instantiate(pointPrefab, spawnPosition, Quaternion.identity);

				SpawnPoint spawnPoint = instance.GetComponent<SpawnPoint>();

				//L'ID correspond au nombre de point de spawn, on commence ici par 0
				spawnPoint.Init(SpawnPoints.Count, hitFloor.point);
				//!FIN


				SpawnPoints.Add(spawnPoint);

				SpawnPointEvent?.Invoke(spawnPoint);

				Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

				//Debug.Log(spawnPoint.Time + " " + spawnPoint._Position + " " + spawnPoint._GameObject.name);
			}

			if (/*Input.GetMouseButtonDown(1)*/Gamepad.Instance.ButtonDownB)
			{
				RemoveLastPoint();
			}

			yield return null;
		}

		SpawnComplete?.Invoke();
	}
}