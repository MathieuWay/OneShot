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
	[SerializeField] private GameObject pointPrefab = null;
	[SerializeField] private int spawnCount = 10;
	[SerializeField] private string[] blockerTags = null;
	[SerializeField] private float detectionDistanceToFloor = 3;
	[SerializeField] private float spawnDistanceToFloor = 0.3f;
	private bool pointSelected;
	private SpawnPoint currentPointSelected;

	public delegate void SpawnDelegate();
	public delegate void SpawnPointDelegate(SpawnPoint spawnPoint);
	public event SpawnPointDelegate SpawnPointEvent;
	public event SpawnPointDelegate GrabPointEvent;
	public event SpawnPointDelegate UngrabPointEvent;
	public event SpawnDelegate SpawnFailEvent;
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

	public void RemoveSelectedPoint()
	{
		if (SpawnPoints.Count <= 0) return;

		Destroy(SpawnPoints[UI_Timeline.Instance.SelectedPoint]._GameObject);
		SpawnPoints.RemoveAt(UI_Timeline.Instance.SelectedPoint);

		UI_Timeline.Instance.RemoveSelectedPoint();

		if(pointSelected)
		{
			pointSelected = false;
			currentPointSelected = null;
		}
	}

	public int GetRemainingPoints()
	{
		return spawnCount - SpawnPoints.Count;
	}

	public void ChangeSpawnPointsOrder()
	{
		SpawnPoints = SpawnPoints.OrderBy(o => o._Time).ToList();
	}

	public void CancelGrab()
	{
		if (!pointSelected) return;

		pointSelected = false;
		currentPointSelected.Unselect();
		currentPointSelected = null;
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
		while (oneShot.LevelController.Instance.phase == oneShot.Phase.Tactical)
		{
			if (/*Input.GetMouseButtonDown(0)*/Gamepad.Instance.ButtonDownA /*&& !UI_PointController.Instance.DraggingPoint()*/)
			{
				//!OLD : Fonctionne uniquement avec une camera en vue orthographique
				//RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(CursorController.Instance.GetPosition()/*Input.mousePosition*/), Vector2.zero);


				RaycastHit2D hit = FireRaycast(CursorController.Instance.GetPosition());

				if (hit.collider == null)
				{
					yield return null;
					continue;
				}

				bool hitBlocker = false;

				//Check tous les tags considérés comme "bloqueur"
				hitBlocker = CheckBlockerTags(hit);

				if (hitBlocker)
				{
					//Tests sur les côtés jusqu'à trouver un emplacement libre
					/*
					float[] testPos = new float[] { 40, -40 };

					for (int i = 0; i < testPos.Length; i++)
					{
						hit = FireRaycast(CursorController.Instance.GetPosition() + new Vector2(testPos[i], 0));
						hitBlocker = CheckBlockerTags(hit);

						if(!hitBlocker)
						{
							//Emplacement trouvé !
							break;
						}
					}
					*/

					if(hitBlocker)
					{
						//Aucun emplacement libre trouvé
						yield return null;
						continue;
					}
				}

				//Impossible de placer un point sur un autre
				if(pointSelected && hit.transform.GetComponent<SpawnPoint>() != null)
				{
					SpawnFailEvent?.Invoke();
					yield return null;
					continue;
				}

				//Point Selection
				if (!pointSelected && hit.transform.GetComponent<SpawnPoint>() != null)
				{
					pointSelected = true;
					
					SpawnPoint point = hit.transform.GetComponent<SpawnPoint>();
					currentPointSelected = point;
					point.Grab();

					GrabPointEvent?.Invoke(point);
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

				//if (hitFloor.transform == null)
				//{
				//	//Si aucune détection de collider, aucun sol n'a été trouvé
				//	yield return null;
				//	continue;
				//}

				//Le point de spawn est décalé suivant Z pour être placé devant la zone Trigger des TP
				Vector3 offset = new Vector3(0, 0, -0.1f);

				Vector3 spawnPosition = hitFloor.collider != null ? hitFloor.point + Vector3.up * spawnDistanceToFloor + offset :
					(Vector3)hit.point;

				Vector3 rootPoint = hitFloor.collider != null ? hitFloor.point : (Vector3)hit.point + new Vector3(0, -spawnDistanceToFloor, 0);

				if(pointSelected)
				{
					//Déplacement Point

					pointSelected = false;

					currentPointSelected.transform.position = spawnPosition;
					currentPointSelected.UpdatePosition(rootPoint);
					currentPointSelected.Select();
					UngrabPointEvent?.Invoke(currentPointSelected);

					currentPointSelected = null;
				}
				else
				{
					if(SpawnPoints.Count >= spawnCount)
					{
						yield return null;
						continue;
					}

					//Ajout Point

					//Instance du point de spawn sur la surface touché
					GameObject instance = Instantiate(pointPrefab, spawnPosition, Quaternion.identity);

					SpawnPoint spawnPoint = instance.GetComponent<SpawnPoint>();

					//L'ID correspond au nombre de point de spawn, on commence ici par 0
					spawnPoint.Init(SpawnPoints.Count, rootPoint);

					SpawnPoints.Add(spawnPoint);

					SpawnPointEvent?.Invoke(spawnPoint);
				}

				Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
			}

			if (/*Input.GetMouseButtonDown(1)*/Gamepad.Instance.ButtonDownB)
			{
				RemoveSelectedPoint();
			}

			yield return null;
		}

		SpawnComplete?.Invoke();
	}

	private bool CheckBlockerTags(RaycastHit2D hit)
	{
		for (int i = 0; i < blockerTags.Length; i++)
		{
			if (hit.transform.CompareTag(blockerTags[i]))
			{
				return true;
			}
		}

		return false;
	}

	private RaycastHit2D FireRaycast(Vector2 startPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(startPos);

		return Physics2D.GetRayIntersection(ray);
	}

	public SpawnPoint SpawnPoint
	{
		get => default;
		set
		{
		}
	}
}