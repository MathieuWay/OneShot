using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class ThrustAttack : Attack
	{
		[Header("THRUST PARAMETERS")]
		[SerializeField] private float detectionRange = 1;
		[SerializeField] private float heightRange = 1;

		public override void Launch()
		{
			Enemy enemy = GetNearestEnemy(new Vector2(detectionRange, heightRange));

			if (enemy != null)
			{
				Enemy[] enemies = GetAllEnemiesInRange(enemy);

				for (int i = 0; i < enemies.Length; i++)
				{
					if(enemies[i].GetComponent<EnemyBase>() != null)
					{
						enemies[i].GetComponent<EnemyBase>().Hit(_AttackName, PlayerBehaviour.Instance.CenterPivot.position);
					}
				}
			}

			SoundManager.Instance.PlaySound("chrona_thrust_attack");
		}

		private Enemy[] GetAllEnemiesInRange(Enemy nearestEnemy)
		{
			Vector2 volumePos = Vector2.zero;

			if (nearestEnemy.Pivot.position.x > PlayerBehaviour.Instance.CenterPivot.position.x)
			{
				volumePos = new Vector2(PlayerBehaviour.Instance.CenterPivot.position.x + detectionRange / 4,
					PlayerBehaviour.Instance.CenterPivot.position.y);
			}
			else
			{
				volumePos = new Vector2(PlayerBehaviour.Instance.CenterPivot.position.x - detectionRange / 4,
					PlayerBehaviour.Instance.CenterPivot.position.y);
			}

			Collider2D[] colliders = Physics2D.OverlapBoxAll(volumePos, new Vector2(detectionRange / 2, heightRange), 0);
			List<Enemy> enemies = new List<Enemy>();

#if UNITY_EDITOR
			StartCoroutine(DebugOverlapBox(1, new Vector2(detectionRange / 2, heightRange), volumePos));
#endif

			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].CompareTag("Enemy"))
				{
					if (colliders[i].GetComponent<Enemy>().isAlive)
					{
						enemies.Add(colliders[i].GetComponent<Enemy>());
					}
				}
			}

			return enemies.ToArray();
		}
	}
}