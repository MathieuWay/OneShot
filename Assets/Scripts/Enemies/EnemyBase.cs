using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public abstract class EnemyBase : MonoBehaviour
	{
		[SerializeField] protected EnemyData enemyData;
		[SerializeField] private GameObject firePrefab;
		[SerializeField] private Transform fireSpawn;

		[Header("DEBUG")]
		[SerializeField] private bool drawGizmos = true;
		private float reactionTime;
		private bool hunt;

		private void Start()
		{
			LevelController.Instance.OnStartCombatPhase += StartHunt;
			TacticsController.Instance.OnPlayerTeleport += ResetReactionTime;

			Enemy enemy = GetComponent<Enemy>();
			enemy.Init(enemyData.Speed);
			enemy.OnKill += StopHunt;
		}

		public virtual void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			GetComponent<Enemy>().Kill();
		}

		protected void PlayerAttackFailed()
		{
			Gamepad.Instance.Vibrate(1, 1, 0.5f);
		}

		private void StopHunt()
		{
			hunt = false;
		}

		private void StartHunt()
		{
			hunt = true;
			StartCoroutine(HuntProcess());
		}
		private IEnumerator HuntProcess()
		{
			reactionTime = enemyData.ReactionTime;
			float fireRate = 0.5f;
			float fireTime = 0;

			while (hunt)
			{
				if(reactionTime <= 0)
				{
					if(fireTime <= 0)
					{
						Fire();
						fireTime = fireRate;
					}

					fireTime -= Time.deltaTime;
					yield return null;
					continue;
				}

				reactionTime -= Time.deltaTime;
				yield return null;
			}
		}

		private void Fire()
		{
			//!TMP: Les ennemis tirent sur le joueur même s'il est mort :)
			//if (PlayerBehaviour.Instance.IsDead) return;

			float distanceToPlayer = Vector2.Distance(fireSpawn.position, PlayerBehaviour.Instance.transform.position);

			if (distanceToPlayer > enemyData.FieldOfView) return;

			Vector2 shootDir = PlayerBehaviour.Instance.CenterPivot.position - fireSpawn.position;

			RaycastHit2D[] hits = Physics2D.RaycastAll(fireSpawn.position, shootDir, enemyData.FieldOfView);

			List<RaycastHit2D> correctHits = new List<RaycastHit2D>();

			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].transform.CompareTag("Obstacle") || hits[i].transform.CompareTag("Player"))
				{
					correctHits.Add(hits[i]);
				}
			}

			float nearestHitDist = Mathf.Infinity;
			RaycastHit2D nearestHit = new RaycastHit2D();

			for (int i = 0; i < correctHits.Count; i++)
			{
				float distanceToHit = Vector2.Distance(correctHits[i].point, fireSpawn.position);

				if(distanceToHit < nearestHitDist)
				{
					nearestHitDist = distanceToHit;
					nearestHit = correctHits[i];
				}
			}

			if (nearestHit.transform.GetComponent<PlayerBehaviour>())
			{
				PlayerBehaviour player = nearestHit.transform.GetComponent<PlayerBehaviour>();
				player.Kill();
				LaunchEffect(fireSpawn.position, nearestHit.point);
			}
		}

		private void LaunchEffect(Vector2 startPoint, Vector2 endPoint)
		{
			GameObject instance = Instantiate(firePrefab);
			LineRenderer line = instance.GetComponent<LineRenderer>();
			line.SetPositions(new Vector3[] { startPoint, endPoint });

			Destroy(instance, 0.1f);
		}

		private void ResetReactionTime()
		{
			reactionTime = enemyData.ReactionTime;
		}

		private void OnDrawGizmosSelected()
		{
			if (!drawGizmos) return;

			Gizmos.color = new Color(1, 0.8f, 0);
			Gizmos.DrawWireSphere(fireSpawn.position, enemyData.FieldOfView);
		}
	}
}