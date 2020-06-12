using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class SlashAttack : Attack
	{
		[Header("SLASH PARAMETERS")]
		[SerializeField] private float detectionRange = 1;
		[SerializeField] private float widthRange = 1;

		public override void Launch()
		{
			Enemy enemy = GetNearestEnemy(new Vector2(widthRange, detectionRange)); 

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
		}

		private Enemy[] GetAllEnemiesInRange(Enemy nearestEnemy)
		{
			Vector2 volumePos = Vector2.zero;

			if(nearestEnemy.Pivot.position.y > PlayerBehaviour.Instance.CenterPivot.position.y)
			{
				volumePos = new Vector2(PlayerBehaviour.Instance.CenterPivot.position.x, 
					PlayerBehaviour.Instance.CenterPivot.position.y + detectionRange / 4);
			}
			else
			{
				volumePos = new Vector2(PlayerBehaviour.Instance.CenterPivot.position.x, 
					PlayerBehaviour.Instance.CenterPivot.position.y - detectionRange / 4);
			}

			Collider2D[] colliders = Physics2D.OverlapBoxAll(volumePos, new Vector2(widthRange, detectionRange / 2), 0);
			List<Enemy> enemies = new List<Enemy>();

#if UNITY_EDITOR
			StartCoroutine(DebugOverlapBox(1, new Vector2(widthRange, detectionRange / 2), volumePos));
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