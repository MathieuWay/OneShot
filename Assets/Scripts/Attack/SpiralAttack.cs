using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace oneShot
{
	public class SpiralAttack : Attack
	{
		[Header("SPIRAL PARAMETERS")]
		[SerializeField] private float radius = 1;

#if UNITY_EDITOR
		private bool isDebug;
#endif

		public override void Launch()
		{
			Collider2D[] hits = Physics2D.OverlapCircleAll(PlayerBehaviour.Instance.CenterPivot.position, radius);

#if UNITY_EDITOR
			StartCoroutine(DrawAttack(1));
#endif

			int enemyKilledCount = 0;

			for (int i = 0; i < hits.Length; i++)
			{
				if(hits[i].CompareTag("Enemy"))
				{
					//DEBUG HIT DISTANCE
					//Debug.Log("HIT: " + hits[i].name + " " + Vector2.Distance(hits[i].transform.position, PlayerBehaviour.Instance.CenterPivot.position));

					Enemy enemy = hits[i].GetComponent<Enemy>();

					if(enemy != null)
					{
						if (enemy.GetComponent<EnemyBase>() != null)
						{
							enemy.GetComponent<EnemyBase>().Hit(_AttackName, PlayerBehaviour.Instance.CenterPivot.position);
						}
						else
						{
							enemy.Kill();
						}

						if(!enemy.isAlive) enemyKilledCount++;
					}
				}
			}

			if(enemyKilledCount >= 2)
			{
				GameTime.Instance.SlowMotion(0.2f, 1.5f);
			}
		}

#if UNITY_EDITOR
		private IEnumerator DrawAttack(float duration)
		{
			isDebug = true;
			yield return new WaitForSeconds(duration);
			isDebug = false;
		}

		private void OnDrawGizmos()
		{
			if (isDebug)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(PlayerBehaviour.Instance.CenterPivot.position, radius);
			}
		}
#endif
	}
}