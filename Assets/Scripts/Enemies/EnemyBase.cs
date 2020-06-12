using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oneShot
{
	public abstract class EnemyBase : MonoBehaviour
	{
		[SerializeField] private EnemyData enemyData = null;
		//[SerializeField] private GameObject firePrefab = null;
		public EnemyData _EnemyData { get => enemyData; }
		protected Enemy enemy;

		public delegate void MyDelegate(EnemyBase enemyBase);
		public static event MyDelegate OnEnemyDeadGetInfo;

		[Header("DEBUG")]
		[SerializeField] private bool drawGizmos = true;
		private float reactionTime;
		private bool hunt;
		

		private void Start()
		{
			LevelController.Instance.OnStartCombatPhase += StartHunt;
			TacticsController.Instance.OnPlayerTeleport += ResetReactionTime;

			enemy = GetComponent<Enemy>();
            enemy.speed = enemyData.Speed;
            enemy.OnKill += EnemyDeath;
		}

		public virtual void Hit(AttackName attackName, Vector2 hitOriginPos)
		{
			GetComponent<Enemy>().Kill();
		}

		protected void PlayerAttackFailed()
		{
			Gamepad.Instance.Vibrate(1, 1, 0.5f);
		}

		private void EnemyDeath()
		{
			hunt = false;
			OnEnemyDeadGetInfo?.Invoke(this);
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

					fireTime -= Time.deltaTime * GameTime.Instance.TimeSpeed;
					yield return null;
					continue;
				}

				reactionTime -= Time.deltaTime * GameTime.Instance.TimeSpeed;
				yield return null;
			}
		}

		private void Fire()
		{
			//!TMP: Les ennemis tirent sur le joueur même s'il est mort :)
			//if (PlayerBehaviour.Instance.IsDead) return;

			float distanceToPlayer = Vector2.Distance(enemy.Pivot.position, PlayerBehaviour.Instance.transform.position);

			//Le joueur doit être à sa portée
			if (distanceToPlayer > enemyData.FieldOfView) return;

			Vector2 shootDir = PlayerBehaviour.Instance.CenterPivot.position - enemy.WeaponPivot.position;

			//Le joueur doit être dans sa direction
			if (Vector2.Dot(shootDir, new Vector2(enemy.GetDirectionX(), 0)) < 0) return;


			RaycastHit2D[] hits = Physics2D.RaycastAll(enemy.WeaponPivot.position, shootDir, enemyData.FieldOfView);

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
				float distanceToHit = Vector2.Distance(correctHits[i].point, enemy.WeaponPivot.position);

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
				LaunchEffect(enemy.WeaponPivot.position, nearestHit.point);
				enemy.Shoot();
			}
		}

		private void LaunchEffect(Vector2 startPoint, Vector2 endPoint)
		{
			/*
			GameObject instance = Instantiate(firePrefab);
			LineRenderer line = instance.GetComponent<LineRenderer>();
			line.SetPositions(new Vector3[] { startPoint, endPoint });
			*/
			Vector2 dir = new Vector2(endPoint.x - startPoint.x, 0);

			Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);

			Debug.DrawLine(endPoint, endPoint + new Vector2(0, 1), Color.red, 5);

			GameObject bullet = VFX_Manager.Instance.GetVFXInstance("bullet", startPoint, targetRot);

			SoundManager.Instance.PlaySound("shotgun", 0.5f);

			//Destroy(instance, 0.1f);
		}

		private void ResetReactionTime()
		{
			reactionTime = enemyData.ReactionTime;
		}

		private void OnDrawGizmosSelected()
		{
			if (!drawGizmos) return;

			if (enemy == null) return;

			Gizmos.color = new Color(1, 0.8f, 0);
			Gizmos.DrawWireSphere(enemy.Pivot.position, enemyData.FieldOfView);

			Debug.DrawRay(enemy.WeaponPivot.position, enemy.WeaponPivot.right * enemyData.FieldOfView, Color.red);
		}
	}
}